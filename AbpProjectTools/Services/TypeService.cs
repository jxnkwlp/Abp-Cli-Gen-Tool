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

namespace AbpProjectTools.Services;

public class TypeService : IDisposable
{
    private readonly string _solutionDir;
    private readonly DirectoryInfo _hostProjectDirectory = null;

    public TypeService(string solutionDir)
    {
        _solutionDir = solutionDir;
        _hostProjectDirectory = FileHelper.GetHostProjectDirectory(_solutionDir);
    }

    public string GetSlutionName()
    {
        var file = Directory.GetFiles(_solutionDir, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

        return Path.GetFileNameWithoutExtension(file);
    }

    private CSharpDecompiler GetDecompiler(string fileName)
    {
        using var module = new PEFile(fileName);

        var resolver = new UniversalAssemblyResolver(fileName, false, module.DetectTargetFrameworkId());
        foreach (var item in UniversalAssemblyResolver.GetGacPaths())
        {
            resolver.AddSearchDirectory(item);
        }

        var webBinDir = _hostProjectDirectory.GetDirectories("Debug", SearchOption.AllDirectories).FirstOrDefault();

        if (webBinDir.EnumerateDirectories().Any())
            webBinDir = webBinDir.EnumerateDirectories().FirstOrDefault();

        resolver.AddSearchDirectory(webBinDir.FullName);

        // add nuget cache 
        var nugetCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".nuget\packages");

        resolver.AddSearchDirectory(nugetCache);

        // new WholeProjectDecompiler

        var decompiler = new CSharpDecompiler(fileName, resolver, new DecompilerSettings());

        return decompiler;
    }

    public EntityDefinitions GetDomain(string name, bool includeDomainProperties = false)
    {
        var domainProject = FileHelper.GetDomainProjectDirectory(_solutionDir);
        var domainSharedProject = FileHelper.GetDomainSharedProjectDirectory(_solutionDir);

        try
        {
            var domainDllFile = domainProject.EnumerateFiles("bin/**.Domain.dll", SearchOption.AllDirectories).FirstOrDefault();

            if (domainDllFile == null)
                throw new Exception("The domain dll not found. Please build project .");

            var decompiler = GetDecompiler(domainDllFile.FullName);
            
            string assemblyName = decompiler.TypeSystem.MainModule.AssemblyName;
            string rootNamespace = decompiler.TypeSystem.MainModule.RootNamespace.Name;

            var domainTypeDefinitions = decompiler.TypeSystem.MainModule.TypeDefinitions;

            var typeDefinitions = domainTypeDefinitions.Where(x => x.Name == name || x.FullName == name);

            if (typeDefinitions.Any() == false)
                throw new Exception($"The type '{name}' not found.");

            if (typeDefinitions.Count() > 1)
                throw new Exception($"The type name '{name}' find more then one. please use full type name.");

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
            var typeNamespaces = new List<string>();

            if (includeDomainProperties)
            {
                var publicPrpoperties = findType.GetProperties(x => x.Accessibility == Accessibility.Public, GetMemberOptions.None);

                foreach (var property in publicPrpoperties)
                {
                    if (property.ReturnType.Namespace != findType.Namespace && !typeNamespaces.Contains(property.ReturnType.Namespace) && property.ReturnType.Namespace != "System")
                    {
                        typeNamespaces.Add(property.ReturnType.Namespace);
                    }

                    var memberInfo = GetTypeMemberInfo(findType, property, domainTypeDefinitions);
                    typeProperties.Add(memberInfo);
                }
            }

            var hasConstructorWithId = findType.GetConstructors(x => x.Parameters.Count == 1 && x.Parameters[0].Type.Name == key).Any();

            // source file
            var csFile = domainProject.EnumerateFiles($"{findType.Name}.cs", SearchOption.AllDirectories).FirstOrDefault();

            if (!csFile.Exists)
            {
                throw new Exception($"The source file '{findType.Name}.cs' not found.");
            }

            return new EntityDefinitions
            {
                TypeKey = key,
                TypeName = findType.Name,
                TypeFullName = findType.FullName,
                TypeNamespace = findType.Namespace,
                FileDirectory = csFile.DirectoryName,
                FileFullName = csFile.FullName,
                FileProjectPath = csFile.DirectoryName.Substring(domainProject.FullName.Length + 1),
                Properties = typeProperties,
                ConstructorWithId = hasConstructorWithId,
                PropertyNamespaces = typeNamespaces,
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

    public AppServiceContractTypeDefinitions GetAppContractTypeDefinitions()
    {
        var appContractProject = FileHelper.GetApplicationContractProjectDirectory(_solutionDir);

        try
        {
            var dllFile = appContractProject.EnumerateFiles("bin/**.Application.Contracts.dll", SearchOption.AllDirectories).FirstOrDefault();

            if (dllFile == null)
                throw new Exception("The application dll not found. Please build project .");

            var decompiler = GetDecompiler(dllFile.FullName);

            string assemblyName = decompiler.TypeSystem.MainModule.AssemblyName;
            string rootNamespace = decompiler.TypeSystem.MainModule.RootNamespace.Name;

            var typeDefinitions = decompiler.TypeSystem.MainModule.TypeDefinitions;

            var types = typeDefinitions.Where(x => x.Name.EndsWith("AppService") || x.Name.EndsWith("Dto"));

            var resultTypes = types.Select(x => new TypeInfo()
            {
                Name = x.Name,
                FullName = x.FullName,
                Namespace = x.Namespace,
            });

            return new AppServiceContractTypeDefinitions
            {
                AllTypes = resultTypes.ToArray(),
                ContractTypes = resultTypes.Where(x => x.Name.EndsWith("AppService")).ToArray(),
                DtoTypes = resultTypes.Where(x => x.Name.EndsWith("Dto")).ToArray(),
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public AppServiceContractDefinition GetAppContractService(string name)
    {
        var appContractProject = FileHelper.GetApplicationContractProjectDirectory(_solutionDir);
        var apiProject = FileHelper.GetHttpControllerProjectDirectory(_solutionDir);

        try
        {
            var csFile = appContractProject.EnumerateFiles($"I{name}AppService.cs", SearchOption.AllDirectories).FirstOrDefault();

            var dllFile = appContractProject.EnumerateFiles("bin/**.Application.Contracts.dll", SearchOption.AllDirectories).FirstOrDefault();

            if (dllFile == null)
                throw new Exception("The dll not found. Please build project .");

            var decompiler = GetDecompiler(dllFile.FullName);

            var appServiceType = decompiler.TypeSystem.MainModule.TypeDefinitions.FirstOrDefault(x => x.Name.StartsWith($"I{name}AppService"));

            if (appServiceType == null)
                throw new Exception($"The application contracts class 'I{name}AppService' of type '{name}' of  not found.");

            var methodTypes = appServiceType.GetMethods();
            var methods = methodTypes.Select(x => new TypeMethodInfo
            {
                Name = x.Name,
                Type = x.ReturnType.Name,
                TypeArguments = x.ReturnType?.TypeArguments?.Select(p => new TypeMemberInfo
                {
                    Name = GetTypeSimpleString(p),
                }).ToList(),
                IsInherited = x.DeclaringType.FullName.StartsWith("System."),
                Params = x.Parameters?.Select(p => new TypeMemberInfo
                {
                    Name = p.Name,
                    Type = p.Type.Name,
                    TypeCode = GetTypeCodeString(p.Type),
                    IsNullable = p.Type.Nullability == Nullability.Nullable,
                }).ToList(),
            });

            return new AppServiceContractDefinition
            {
                ServiceName = appServiceType.Name[1..],
                Name = appServiceType.Name[1..].Replace("AppService", null),
                Namespace = appServiceType.Namespace,
                FileProjectPath = csFile.DirectoryName.Substring(appContractProject.FullName.Length + 1),
                Methods = methods.ToList(),
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public HttpControllerTypeDefinitions GetHttpControllerTypeDefinitions()
    {
        var project = FileHelper.GetHttpControllerProjectDirectory(_solutionDir);

        try
        {
            var dllFile = project.EnumerateFiles("bin/**.HttpApi.dll", SearchOption.AllDirectories).FirstOrDefault();

            if (dllFile == null)
                throw new Exception("The HttpApi dll not found. Please build project .");

            var decompiler = GetDecompiler(dllFile.FullName);

            string assemblyName = decompiler.TypeSystem.MainModule.AssemblyName;
            string rootNamespace = decompiler.TypeSystem.MainModule.RootNamespace.Name;

            var typeDefinitions = decompiler.TypeSystem.MainModule.TypeDefinitions;

            var baseControllerType = typeDefinitions.FirstOrDefault(x => x.DirectBaseTypes.Any(b => b.Name == "AbpControllerBase"));

            var importNamespace = new List<string>();

            if (baseControllerType.Namespace != rootNamespace)
                importNamespace.Add(baseControllerType.Namespace);

            return new HttpControllerTypeDefinitions
            {
                BaseNamespace = rootNamespace,
                BaseControllerType = baseControllerType.Name,
                ImportNamespaces = importNamespace,
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static TypeMemberInfo GetTypeMemberInfo(ITypeDefinition typeDefinition, IProperty property, IEnumerable<ITypeDefinition> allTypeDefinitions)
    {
        var typeFullName = property.ReturnType.FullName;
        var innerType = property.ReturnType;
        // 
        if (property.ReturnType.FullName != "System.Nullable" && property.ReturnType.TypeArguments.Count > 0 && property.ReturnType.FullName != property.ReturnType.TypeArguments[0].FullName)
        {
            innerType = property.ReturnType.TypeArguments[0];
        }

        var isClass = allTypeDefinitions.Any(x => x.FullName == innerType.FullName || x.FullName == typeFullName);
        // var isThirdPartyClass = !isClass && !domainTypeDefinitions.Any(x => x.Name == item.ReturnType.Name);

        return new TypeMemberInfo
        {
            Name = property.Name,
            TypeCode = GetTypeCodeString(property.ReturnType),
            Type = typeFullName,
            InnerType = innerType.Name,
            InnerTypeFullName = innerType.FullName,
            IsNullable = property.ReturnType.FullName == "System.Nullable",
            IsRequired = property.ReturnType.Nullability == Nullability.NotNullable,
            IsInherited = property.DeclaringTypeDefinition != typeDefinition,
            IsWrite = property.CanSet,
            IsClass = isClass,
        };
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
        var typeCode = type.GetTypeCode();

        if (typeCode == TypeCode.Empty)
        {
            if (type.TypeArguments.Count > 0)
            {
                if (type.FullName == "System.Nullable")
                    return type.TypeArguments[0].Name;
                else
                {
                    var innerTypes = string.Join(", ", type.TypeArguments.Select(x => $"{x.Name}"));
                    return $"{type.Name}<{innerTypes}>";
                }
            }

            if (type.ReflectionName == "System.String")
            {
                return "string";
            }

            return type.Name;
        }

        switch (typeCode)
        {
            //case TypeCode.Empty: return "";
            case TypeCode.Object: return "object";
            //case TypeCode.DBNull: return "";
            case TypeCode.Boolean: return "bool";
            case TypeCode.Char: return "char";
            case TypeCode.SByte: return "sbyte";
            case TypeCode.Byte: return "byte";
            case TypeCode.Int16: return "short";
            case TypeCode.UInt16: return "ushort";
            case TypeCode.Int32: return "int";
            case TypeCode.UInt32: return "uint";
            case TypeCode.Int64: return "long";
            case TypeCode.UInt64: return "ulong";
            case TypeCode.Single: return "float";
            case TypeCode.Double: return "double";
            case TypeCode.Decimal: return "decimal";
            case TypeCode.DateTime: return "DateTime";
            case TypeCode.String: return "string";
            default:
                throw new Exception($"Unknow type code '{type.GetTypeCode()}'");
        }
    }

    public void Dispose()
    {
        
    }
}

internal class FileResolver : UniversalAssemblyResolver
{
    public FileResolver(string mainAssemblyFileName, bool throwOnError, string targetFramework, string runtimePack = null, PEStreamOptions streamOptions = PEStreamOptions.Default, MetadataReaderOptions metadataOptions = MetadataReaderOptions.ApplyWindowsRuntimeProjections) : base(mainAssemblyFileName, throwOnError, targetFramework, runtimePack, streamOptions, metadataOptions)
    {
    }

}

/// <summary>
///  The type information of class
/// </summary>
public class TypeInfo
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Namespace { get; set; }

    public override string ToString()
    {
        return FullName;
    }
}

public class TypeMemberInfo
{
    /// <summary>
    ///  The member name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    ///  The member type
    /// </summary>
    public string Type { get; set; }
    public string TypeCode { get; set; }
    /// <summary>
    ///  the member inner type from type if exists.
    /// </summary>
    public string InnerTypeFullName { get; set; }
    public string InnerType { get; set; }
    public bool IsInherited { get; set; }
    public bool IsNullable { get; set; }
    public bool IsWrite { get; set; }
    public bool IsRequired { get; set; }
    public bool IsClass { get; set; }

    public override string ToString()
    {
        return $"{Type} {Name}";
    }
}

public class TypeMethodInfo
{
    public string Name { get; set; }

    public string Type { get; set; }

    public bool IsInherited { get; set; }

    public IList<TypeMemberInfo> TypeArguments { get; set; }

    public IList<TypeMemberInfo> Params { get; set; }
}
