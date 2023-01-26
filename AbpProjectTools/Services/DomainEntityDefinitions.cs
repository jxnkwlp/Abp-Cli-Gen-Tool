using System.Collections.Generic;

namespace AbpProjectTools.Services;

public class DomainEntityDefinitions
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

}
