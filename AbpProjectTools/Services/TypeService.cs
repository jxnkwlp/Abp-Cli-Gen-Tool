using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace AbpProjectTools
{
    public class TypeService
    {
        private readonly string _solutionDir;
        private readonly DirectoryInfo _hostProjectDirectory = null;

        public TypeService(string solutionDir)
        {
            _solutionDir = solutionDir;
            _hostProjectDirectory = FileHelper.GetWebProjectDirectory(_solutionDir);
        }

        private CSharpDecompiler GetDecompiler(string fileName)
        {
            var module = new PEFile(fileName);

            var resolver = new UniversalAssemblyResolver(fileName, false, module.DetectTargetFrameworkId());
            foreach (var item in UniversalAssemblyResolver.GetGacPaths())
            {
                resolver.AddSearchDirectory(item);
            }

            var webBinDir = _hostProjectDirectory.GetDirectories("Debug", SearchOption.AllDirectories).FirstOrDefault();

            if (webBinDir.EnumerateDirectories().Any())
            {
                webBinDir = webBinDir.EnumerateDirectories().FirstOrDefault();
            }

            resolver.AddSearchDirectory(webBinDir.FullName);

            // add nuget cache 
            var nugetCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".nuget\packages");

            resolver.AddSearchDirectory(nugetCache);

            // new WholeProjectDecompiler

            var decompiler = new CSharpDecompiler(fileName, resolver, new DecompilerSettings());

            return decompiler;
        }

        public DomainEntityDefinitions GetDomain(string name, bool includeDomainProperties = false)
        {
            var domainProject = FileHelper.GetDomainProjectDirectory(_solutionDir);

            var csFile = domainProject.EnumerateFiles($"{name}.cs", SearchOption.AllDirectories).FirstOrDefault();

            try
            {
                var dllFile = domainProject.EnumerateFiles("bin/**.Domain.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (dllFile == null)
                    throw new Exception("The dll not found. Please build project .");

                var decompiler = GetDecompiler(dllFile.FullName);

                string assemblyName = decompiler.TypeSystem.MainModule.AssemblyName;
                string rootNamespace = decompiler.TypeSystem.MainModule.RootNamespace.Name;

                var typeDefinitions = decompiler.TypeSystem.MainModule.TypeDefinitions.Where(x => x.Name == name);

                if (typeDefinitions.Any() == false)
                {
                    throw new Exception($"The type '{name}' not found.");
                }

                if (typeDefinitions.Count() > 1)
                {
                    throw new Exception($"The type name '{name}' find more then one. please use full type name.");
                }

                var findType = typeDefinitions.First();

                string key = string.Empty;

                foreach (var item in findType.DirectBaseTypes)
                {
                    if (item.TypeArguments.Count > 0)
                    {
                        key = item.TypeArguments[0].Name;
                        break;
                    }
                }

                var typeProperties = new List<TypeMemberInfo>();

                if (includeDomainProperties)
                {
                    var publicPrpoperties = findType.GetProperties(x => x.Accessibility == Accessibility.Public, GetMemberOptions.None);

                    foreach (var item in publicPrpoperties)
                    {
                        typeProperties.Add(new TypeMemberInfo
                        {
                            Name = item.Name,
                            Type = GetTypeCodeString(item.ReturnType),
                            IsNullable = item.ReturnType.Nullability == Nullability.Nullable,
                            IsInherited = item.DeclaringTypeDefinition != findType
                        });
                    }
                }

                return new DomainEntityDefinitions
                {
                    TypeKey = key,
                    TypeName = findType.Name,
                    TypeFullName = findType.FullName,
                    TypeNamespace = findType.Namespace,
                    FileDirectory = csFile.DirectoryName,
                    FileFullName = csFile.FullName,
                    FileProjectPath = csFile.DirectoryName.Substring(domainProject.FullName.Length + 1),

                    Properties = typeProperties,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EfCoreContextDefinitions GetEfCore()
        {
            var efProject = FileHelper.GetEntityFrameworkCoreProjectDirectory(_solutionDir);

            try
            {
                var efDllFile = efProject.EnumerateFiles("bin/**.EntityFrameworkCore.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (efDllFile == null)
                    throw new Exception("The dll not found. Please build project .");

                var csFile = efProject.EnumerateFiles("*DbContext.cs", SearchOption.AllDirectories).FirstOrDefault();

                var decompiler = GetDecompiler(efDllFile.FullName);

                var types = decompiler.TypeSystem.GetAllTypeDefinitions();

                var findTypes = decompiler.TypeSystem.MainModule.TypeDefinitions.Where(x => x.FullName.EndsWith("DbContext"));

                if (findTypes.Any() == false)
                    throw new Exception($"No dbcontext found.");

                var findType = findTypes.First();

                return new EfCoreContextDefinitions
                {
                    TypeName = findType.Name,
                    TypeNamespace = findType.Namespace,
                    TypeFullName = findType.FullName,
                    FileDirectoryName = csFile.DirectoryName,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AppServiceDefinition GetAppServiceContract(string name)
        {
            var appContractProject = FileHelper.GetApplicationContractProjectDirectory(_solutionDir);
            var apiProject = FileHelper.GetHttpControllerProjectDirectory(_solutionDir);

            try
            {
                var dllFile = appContractProject.EnumerateFiles("bin/**.Application.Contracts.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (dllFile == null)
                    throw new Exception("The dll not found. Please build project .");

                var decompiler = GetDecompiler(dllFile.FullName);

                var appServiceType = decompiler.TypeSystem.MainModule.TypeDefinitions.FirstOrDefault(x => x.Name.StartsWith($"I{name}AppService"));

                if (appServiceType == null)
                    throw new Exception($"The application contracts type '{name}' not found.");

                var methodTypes = appServiceType.GetMethods();
                var methods = methodTypes.Select(x => new TypeMethodInfo
                {
                    Name = x.Name,
                    ReturnType = x.ReturnType.Name,
                    ReturnTypeArguments = x.ReturnType?.TypeArguments?.Select(p => new TypeMemberInfo
                    {
                        Name = GetTypeSimpleString(p),
                    }).ToList(),
                    IsInherited = x.DeclaringType.FullName.StartsWith("System."),
                    Params = x.Parameters?.Select(p => new TypeMemberInfo
                    {
                        Name = p.Name,
                        Type = GetTypeCodeString(p.Type),
                        IsNullable = p.Type.Nullability == Nullability.Nullable,
                    }).ToList(),
                });

                return new AppServiceDefinition
                {
                    Methods = methods.ToList(),
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string GetTypeSimpleString(IType sourceType)
        {
            string LoopTypeArguments(string parentTypeName, IType type2)
            {
                if (type2.TypeArguments.Count > 0)
                {
                    parentTypeName += "<";
                    foreach (var item in type2.TypeArguments)
                    {
                        parentTypeName += item.Name;
                        parentTypeName = LoopTypeArguments(parentTypeName, item);
                    }
                    parentTypeName += ">";
                }

                return parentTypeName;
            }

            return LoopTypeArguments(sourceType.Name, sourceType);
        }

        private static string GetTypeCodeString(IType type)
        {
            if (type.GetTypeCode() == TypeCode.Empty)
            {
                if (type.TypeArguments.Count > 0)
                {
                    return type.TypeArguments[0].Name;
                }

                return type.Name;
            }

            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean: return "bool";
                case TypeCode.Char: return "char";
                case TypeCode.SByte: return "byte";
                case TypeCode.Byte: return "bool";
                case TypeCode.Int16: return "short";
                case TypeCode.Int32: return "int";
                case TypeCode.UInt64: return "ulong";
                case TypeCode.DateTime: return "datetime";
                case TypeCode.Decimal: return "decimal";
                case TypeCode.Double: return "double";
                case TypeCode.String: return "string";
                default:
                    throw new Exception($"Unknow type code '{type.GetTypeCode()}'. ");
            }
        }
    }

    internal class FileResolver : UniversalAssemblyResolver
    {
        public FileResolver(string mainAssemblyFileName, bool throwOnError, string targetFramework, string runtimePack = null, PEStreamOptions streamOptions = PEStreamOptions.Default, MetadataReaderOptions metadataOptions = MetadataReaderOptions.ApplyWindowsRuntimeProjections) : base(mainAssemblyFileName, throwOnError, targetFramework, runtimePack, streamOptions, metadataOptions)
        {
        }

    }

    public class TypeMemberInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsInherited { get; set; }
        public bool IsNullable { get; set; }
    }

    public class TypeMethodInfo
    {
        public string Name { get; set; }

        public string ReturnType { get; set; }

        public IList<TypeMemberInfo> ReturnTypeArguments { get; set; }

        public bool IsInherited { get; set; }

        public IList<TypeMemberInfo> Params { get; set; }
    }

    public class AppServiceRouteInfo
    {
        public AppServiceRouteInfo(string method, string url = null)
        {
            Method = method;
            Url = url;
        }

        public string Method { get; set; }
        public string Url { get; set; }
    }

    public class AppServiceDefinition
    {
        public IList<TypeMethodInfo> Methods { get; set; }
    }
}
