using AbpProjectTools.Models;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AbpProjectTools.Services;

public class TypeService : IDisposable
{
    private static readonly Dictionary<string, ClassTypeInfoModel> _cache = new Dictionary<string, ClassTypeInfoModel>();

    private readonly DirectoryInfo _sluDirectory = null;

    public TypeService(string sluDirectory)
    {
        _sluDirectory = new DirectoryInfo(sluDirectory);
    }

    private CSharpDecompiler GetDecompiler(string fileName)
    {
        using var module = new PEFile(fileName);

        var resolver = new UniversalAssemblyResolver(fileName, false, module.DetectTargetFrameworkId());
        foreach (var item in UniversalAssemblyResolver.GetGacPaths())
        {
            resolver.AddSearchDirectory(item);
        }

        if (_sluDirectory.Exists)
        {
            var allBinDirs = _sluDirectory.EnumerateDirectories("Debug", SearchOption.AllDirectories);

            foreach (var item in allBinDirs)
            {
                resolver.AddSearchDirectory(item.FullName);
            }
        }

        // add nuget cache 
        var nugetCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");

        resolver.AddSearchDirectory(nugetCache);

        // new WholeProjectDecompiler

        var decompiler = new CSharpDecompiler(fileName, resolver, new DecompilerSettings());

        return decompiler;
    }

    public ClassTypeInfoModel GetDomainInfo(string dllFile, string typeName)
    {
        if (_cache.ContainsKey(typeName))
            return _cache[typeName];

        var result = GetTypeInfo(dllFile, typeName);
        _cache[typeName] = result;
        return result;
    }

    public ClassTypeInfoModel GetRepoDbContext(string dllFile)
    {
        var decompiler = GetDecompiler(dllFile);

        string assemblyName = decompiler.TypeSystem.MainModule.AssemblyName;
        var allTypeDefinitions = decompiler.TypeSystem.MainModule.TypeDefinitions;

        var findDbContext = allTypeDefinitions.FirstOrDefault(x => x.GetAllBaseTypeDefinitions().Any(e => e.Name == "IIdentityDbContext" || e.Name == "IAbpMongoDbContext") && x.Kind == TypeKind.Interface);
        if (findDbContext != null)
            return new ClassTypeInfoModel
            {
                Name = findDbContext.Name,
                FullName = findDbContext.FullName,
                Namespace = findDbContext.Namespace,
            };

        findDbContext = allTypeDefinitions.FirstOrDefault(x => x.GetAllBaseTypeDefinitions().Any(e => e.Name == "IIdentityDbContext" || e.Name == "IAbpMongoDbContext") && x.Kind == TypeKind.Class);
        if (findDbContext != null)
            return new ClassTypeInfoModel
            {
                Name = findDbContext.Name,
                FullName = findDbContext.FullName,
                Namespace = findDbContext.Namespace,
            };

        throw new Exception("The DbContext not found.");
    }

    public ClassTypeInfoModel GetAppContract(string dllFile, string name)
    {
        var decompiler = GetDecompiler(dllFile);

        string assemblyName = decompiler.TypeSystem.MainModule.AssemblyName;
        var allTypeDefinitions = decompiler.TypeSystem.MainModule.TypeDefinitions;

        var find = allTypeDefinitions.FirstOrDefault(x => x.Name.Equals($"I{name}AppService", StringComparison.InvariantCultureIgnoreCase));

        if (find == null)
        {
            find = allTypeDefinitions.FirstOrDefault(x => x.Kind == TypeKind.Interface && x.GetAllBaseTypeDefinitions().Any(e => e.FullName == "Volo.Abp.Application.Services.IApplicationService") && x.Name.Contains(name));
        }

        if (find == null)
        {
            throw new Exception($"The name of '{name}' app service contract not found.");
        }

        var methods = GetMethods(find);

        return new ClassTypeInfoModel
        {
            Name = find.Name,
            FullName = find.FullName,
            Namespace = find.Namespace,
            Methods = methods,
        };
    }

    public ClassTypeInfoModel GetHttpControllerBaseType(string dllFile)
    {
        var decompiler = GetDecompiler(dllFile);

        string assemblyName = decompiler.TypeSystem.MainModule.AssemblyName;
        var allTypeDefinitions = decompiler.TypeSystem.MainModule.TypeDefinitions;

        var find = allTypeDefinitions.FirstOrDefault(x => x.IsAbstract && x.DirectBaseTypes.Any(e => e.FullName == "Volo.Abp.AspNetCore.Mvc.AbpControllerBase"));

        if (find == null)
            return null;

        return new ClassTypeInfoModel
        {
            Name = find.Name,
            FullName = find.FullName,
            Namespace = find.Namespace,
        };
    }

    private ClassTypeInfoModel GetTypeInfo(string dllFile, string typeName)
    {
        var decompiler = GetDecompiler(dllFile);

        //string assemblyName = decompiler.TypeSystem.MainModule.AssemblyName;
        //var allTypeDefinitions = decompiler.TypeSystem.MainModule;

        return GetTypeInfo(decompiler.TypeSystem.MainModule, typeName);
    }

    private ClassTypeInfoModel GetTypeInfo(IModule module, string typeName)
    {
        var result = module.TypeDefinitions.Where(x => x.Name == typeName || x.FullName == typeName);

        if (!result.Any())
            throw new Exception($"The type '{typeName}' not found.");

        if (result.Count() > 1)
            throw new Exception($"The type name '{typeName}' more then one. please use the full type name.");

        var findType = result.First();

        var baseTypes = GetBaseTypes(findType);
        var domainKeyType = FindDomainKey(findType);

        var members = GetMembers(findType, result);

        var references = new List<ClassTypeInfoModel>();

        foreach (var item in members.Where(x => x.IsClass && !x.IsInherit))
        {
            references.Add(GetTypeInfo(item.TypeModule, item.TypeFullName));
        }

        return new ClassTypeInfoModel
        {
            Key = domainKeyType,
            Name = findType.Name,
            FullName = findType.FullName,
            Namespace = findType.Namespace,
            BaseType = baseTypes.BaseType,
            BaseAbpType = baseTypes.BaseAbpType,
            IsAggregatedType = baseTypes.IsAbpAggregateRoot,
            IsAbpType = baseTypes.IsAbpDomainType,
            Members = members,
            References = references,
        };
    }


    private ClassBaseTypeInfo GetBaseTypes(ITypeDefinition typeDefinition)
    {
        var allbaseTypes = typeDefinition.GetAllBaseTypeDefinitions().Where(x => x.FullName != typeDefinition.FullName).Reverse().ToArray();

        return new ClassBaseTypeInfo
        {
            IsAbpAggregateRoot = allbaseTypes.Any(x => x.Name == "IAggregateRoot"),
            IsAbpDomainType = allbaseTypes.Any(x => x.Name == "IEntity"),
            BaseAbpType = allbaseTypes.FirstOrDefault(x => x.FullName.StartsWith("Volo.Abp."))?.Name,
            BaseType = allbaseTypes.FirstOrDefault().Name,
        };
    }

    //private string FindBaseDomainType(ITypeDefinition typeDefinition)
    //{
    //    foreach (var item in typeDefinition.DirectBaseTypes.Where(x => !x.FullName.StartsWith("System.")))
    //    {
    //        if (item.FullName.StartsWith("Volo.Abp."))
    //        {
    //            return item.Name;
    //        }
    //        else
    //        {
    //            return FindBaseDomainType(item.GetDefinition());
    //        }
    //    }

    //    return null;
    //}

    private string FindDomainKey(ITypeDefinition typeDefinition)
    {
        foreach (var item in typeDefinition.DirectBaseTypes.Where(x => !x.FullName.StartsWith("System.")))
        {
            if (item.FullName.StartsWith("Volo.Abp.") && item.TypeParameterCount > 0)
            {
                return item.TypeArguments[0].Name;
            }
            else
            {
                return FindDomainKey(item.GetDefinition());
            }
        }

        return null;
    }

    public List<MethodTypeInfoModel> GetMethods(ITypeDefinition currentType)
    {
        var list = new List<MethodTypeInfoModel>();

        foreach (var item in currentType.Methods.Where(x => x.Accessibility == Accessibility.Public))
        {
            var members = item.Parameters.Select(x => new MethodMemberTypeInfoModel
            {
                IsClass = !x.Type.FullName.StartsWith("System."),
                Name = x.Name,
                TypeCode = GetTypeCodeString(x.Type),
                TypeName = x.Type.Name,
            }).ToList();

            bool isAsync = item.ReturnType.FullName == "System.Threading.Tasks.Task";

            var m = new MethodTypeInfoModel
            {
                Name = item.Name,
                IsAsync = isAsync,
                ReturnTypeName = item.ReturnType.FullName,
                ReturnTypeCode = GetTypeCodeString(item.ReturnType),
                Members = members
            };

            list.Add(m);
        }

        return list;
    }

    //private MethodReturnTypeInfo GetReturnType(IType type)
    //{
    //    string GetTypeArgumentName(IType currentType)
    //    {
    //        if (currentType.TypeArguments.Any())
    //            return GetTypeArgumentName(currentType.TypeArguments[0]);
    //        else
    //            return currentType.Name;
    //    }

    //    var isAsync = type.FullName == "System.Threading.Tasks.Task";
    //    // TypeArguments = {ICSharpCode.Decompiler.TypeSystem.IType[1]}
    //    if (isAsync)
    //    {
    //        var code = GetTypeArgumentName(type.TypeArguments[0]);
    //    }

    //    return new MethodReturnTypeInfo()
    //    {
    //        FullTypeName = type.FullName,
    //        IsAsync = type.FullName == "System.Threading.Tasks.Task",
    //        TypeCode = isAsync ? GetTypeArgumentName(type.TypeArguments[0]) : type.Name,
    //    };
    //}

    public List<MemberTypeInfoModel> GetMembers(ITypeDefinition currentType, IEnumerable<ITypeDefinition> allTypeDefinitions)
    {
        var typeNamespaces = new List<string>();
        var members = new List<MemberTypeInfoModel>();
        var list = currentType.GetProperties(x => x.Accessibility == Accessibility.Public, GetMemberOptions.None);

        foreach (var property in list)
        {
            if (property.ReturnType.Namespace != currentType.Namespace && !typeNamespaces.Contains(property.ReturnType.Namespace) && property.ReturnType.Namespace.StartsWith("System."))
            {
                typeNamespaces.Add(property.ReturnType.Namespace);
            }

            var memberInfo = GetTypeMemberInfo(currentType, property, allTypeDefinitions);
            members.Add(memberInfo);
        }

        return members;
    }

    private static MemberTypeInfoModel GetTypeMemberInfo(ITypeDefinition typeDefinition, IProperty property, IEnumerable<ITypeDefinition> allTypeDefinitions)
    {
        var returnType = property.ReturnType;

        // 
        if (property.ReturnType.FullName != "System.Nullable" && property.ReturnType.TypeArguments.Count > 0 && property.ReturnType.FullName != property.ReturnType.TypeArguments[0].FullName)
        {
            returnType = property.ReturnType.TypeArguments[0];
        }

        var isInherit = property.Namespace != typeDefinition.Namespace;
        var returnTypeFullName = property.ReturnType.FullName;
        var isClass = !property.ReturnType.FullName.StartsWith("System.");
        var isNullable = property.ReturnType.FullName == "System.Nullable";

        return new MemberTypeInfoModel
        {
            MemberName = property.Name,

            TypeCode = GetTypeCodeString(property.ReturnType),
            TypeName = returnType.Name,
            TypeFullName = returnTypeFullName,
            TypeNamespace = returnType.Namespace,

            TypeModule = property.ReturnType.GetDefinition().ParentModule,

            CanSet = property.Setter?.Accessibility == Accessibility.Public,
            CanGet = property.CanGet,

            IsNullable = isNullable,
            IsClass = isClass,
            IsInherit = isInherit,
        };
    }

    private static string GetTypeCodeString(IType type)
    {
        var typeCode = type.GetTypeCode();

        if (typeCode == TypeCode.Empty)
        {
            if (type.TypeArguments.Count > 0)
            {
                if (type.FullName == "System.Nullable")
                    return GetTypeCodeString(type.TypeArguments[0]);
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(type.Name + "<");
                    foreach (var item in type.TypeArguments)
                    {
                        sb.Append(GetTypeCodeString(item));
                    }
                    return sb.Append(">").ToString();
                    //var innerTypes = string.Join(", ", type.TypeArguments.Select(x => $"{x.Name}"));
                    //return $"{type.Name}<{innerTypes}>";
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

    private class ClassBaseTypeInfo
    {
        public bool IsAbpDomainType { get; set; }
        public bool IsAbpAggregateRoot { get; set; }
        public string BaseType { get; set; }
        public string BaseAbpType { get; set; }
    }

    private class MethodReturnTypeInfo
    {
        public bool IsAsync { get; set; }
        public string FullTypeName { get; set; }
        public string TypeCode { get; set; }
    }

    public void Dispose()
    {

    }
}
