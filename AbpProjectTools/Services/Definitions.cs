using System.Collections.Generic;

namespace AbpProjectTools.Services;

public class DBRepositoryDefinitions
{
    public string TypeName { get; set; }

    public string TypeFullName { get; set; }

    public string TypeNamespace { get; set; }

    public string FileDirectoryName { get; set; }

}


public class EntityDefinitions
{
    ///// <summary>
    /////  The Project name 
    ///// </summary>
    //public string ProjectName { get; set; }

    /// <summary>
    ///  The entity type key 
    /// </summary>
    public string TypeKey { get; set; }

    /// <summary>
    ///  The entity type short name 
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    ///  The entity type full name
    /// </summary>
    public string TypeFullName { get; set; }

    /// <summary>
    ///  The entity type namespace
    /// </summary>
    public string TypeNamespace { get; set; }

    /// <summary>
    ///  The entity type file directotry
    /// </summary>
    public string FileDirectory { get; set; }

    /// <summary>
    ///  The file path 
    /// </summary>
    public string FileFullName { get; set; }

    public string FileProjectPath { get; set; }

    public IList<TypeMemberInfo> Properties { get; set; }

    public bool ConstructorWithId { get; set; }
    public List<string> PropertyNamespaces { get; set; }
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

public class AppServiceContractDefinition
{
    public IList<TypeMethodInfo> Methods { get; set; }
    public string Namespace { get; set; }
    public string FileProjectPath { get; set; }
    public string ServiceName { get; set; }
    public string Name { get; set; }
}

public class AppServiceContractTypeDefinitions
{
    public IList<TypeInfo> AllTypes { get; set; }
    public IList<TypeInfo> DtoTypes { get; set; }
    public IList<TypeInfo> ContractTypes { get; set; }
}

public class AppServiceDefinition
{
    public string Namespace { get; set; }
    public string FileProjectPath { get; set; }
    public string AppName { get; set; }
}

public class HttpControllerTypeDefinitions
{
    public string BaseControllerType { get; set; }
    public string BaseNamespace { get; set; }
    public List<string> ImportNamespaces { get; set; }
}
