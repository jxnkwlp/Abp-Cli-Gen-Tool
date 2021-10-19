using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;

namespace AbpProjectTools
{
    public static class TypeHelper
    {
        public static DomainDefinitions GetDomain(DirectoryInfo project, string name)
        {
            try
            {
                var dllFile = project.EnumerateFiles("bin/**.Domain.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (dllFile == null)
                    throw new Exception("The dll not found. Please build project .");

                var decompiler = new CSharpDecompiler(dllFile.FullName, new DecompilerSettings());

                var types = decompiler.TypeSystem.GetAllTypeDefinitions();

                var findType = types.FirstOrDefault(x => x.FullName.EndsWith(name));

                if (findType == null)
                    throw new Exception($"The type '{name}' not found.");

                string key = string.Empty;

                foreach (var item in findType.DirectBaseTypes)
                {
                    if (item.TypeArguments.Count > 0)
                    {
                        key = item.TypeArguments[0].Name;
                        break;
                    }
                }

                return new DomainDefinitions
                {
                    TypeKey = key,
                    TypeName = findType.Name,
                    TypeFullName = findType.FullName,
                    TypeNamespce = findType.Namespace,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static EfCoreContextDefinitions GetEfCore(DirectoryInfo project)
        {
            try
            {
                var dllFile = project.EnumerateFiles("bin/**.EntityFrameworkCore.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (dllFile == null)
                    throw new Exception("The dll not found. Please build project .");

                var decompiler = new CSharpDecompiler(dllFile.FullName, new DecompilerSettings());

                var types = decompiler.TypeSystem.GetAllTypeDefinitions();

                var findTypes = types.Where(x => x.FullName.EndsWith("DbContext"));

                if (findTypes.Any() == false)
                    throw new Exception($"No dbcontext found.");

                var findType = findTypes.First();

                //if (findTypes.Any(x => x.DeclaringType.inter)
                //{
                //    findType = findTypes.First(x => x.IsInterface);
                //}
                //else
                //{
                //    throw new Exception("Multi dbcontext type found.");
                //}

                return new EfCoreContextDefinitions
                {
                    TypeName = findType.Name,
                    TypeNamespce = findType.Namespace,
                    TypeFullName = findType.FullName,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetProptiesString(DirectoryInfo domainProject, string name)
        {
            try
            {
                var dllFile = domainProject.EnumerateFiles("bin/**.Domain.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (dllFile == null)
                    throw new Exception("The dll not found. Please build project .");

                var decompiler = new CSharpDecompiler(dllFile.FullName, new DecompilerSettings());

                var types = decompiler.TypeSystem.GetAllTypeDefinitions();

                var findType = types.FirstOrDefault(x => x.FullName.EndsWith(name));

                if (findType == null)
                    throw new Exception($"The type '{name}' not found.");

                StringBuilder sb = new StringBuilder();

                foreach (var item in findType.Properties)
                {
                    sb.AppendLine($"\t\tpublic {GetTypeString(item.ReturnType)} {item.Name} {{ get; set; }} ");
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }

            string GetTypeString(IType type)
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

        public static string GenerateHttpApiController(DirectoryInfo appContractProject, string webProjectPath, string name)
        {
            var webDllFile = Directory.GetFiles(webProjectPath, "*.Web.dll", SearchOption.AllDirectories).FirstOrDefault();

            var webBin = Path.GetDirectoryName(webDllFile);

            try
            {
                var dllFile = appContractProject.EnumerateFiles("bin/**.Application.Contracts.dll", SearchOption.AllDirectories).FirstOrDefault();

                if (dllFile == null)
                    throw new Exception("The dll not found. Please build project .");

                var resolver = new UniversalAssemblyResolver(dllFile.FullName, true, ".NETCoreApp");
                foreach (var item in UniversalAssemblyResolver.GetGacPaths())
                {
                    resolver.AddSearchDirectory(item);
                }

                resolver.AddSearchDirectory(webBin);

                // add nuget cache 
                var nugetCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".nuget\packages");

                resolver.AddSearchDirectory(nugetCache);

                var decompiler = new CSharpDecompiler(dllFile.FullName, resolver, new DecompilerSettings());

                var types = decompiler.TypeSystem.GetAllTypeDefinitions();

                var findType = types.FirstOrDefault(x => x.FullName.EndsWith($"I{name}AppService"));

                if (findType == null)
                    throw new Exception($"The application contracts type '{name}' not found.");

                StringBuilder sb = new StringBuilder();

                foreach (var item in findType.Methods.Where(x => x.Accessibility == Accessibility.Public))
                {
                    sb.AppendLine($"\t\tpublic {item.ReturnType.Name} {item.Name} {{ ");
                    sb.AppendLine($"\t\t\t_service.{item.Name}(); ");
                    sb.AppendLine($"\t\t}}  ");
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    internal class FileResolver : UniversalAssemblyResolver
    {
        public FileResolver(string mainAssemblyFileName, bool throwOnError, string targetFramework, string runtimePack = null, PEStreamOptions streamOptions = PEStreamOptions.Default, MetadataReaderOptions metadataOptions = MetadataReaderOptions.ApplyWindowsRuntimeProjections) : base(mainAssemblyFileName, throwOnError, targetFramework, runtimePack, streamOptions, metadataOptions)
        {
        }

    }
}
