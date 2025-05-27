namespace AbpProjectTools.Models;

public class BackendCodeGenerateGlobalOptions
{
    /// <summary>
    ///  The directory of the solution project
    /// </summary>
    public string SluDir { get; set; }
    /// <summary>
    /// The name of the solution project
    /// </summary>
    public string SluName { get; set; }
    /// <summary>
    /// Force to overwrite the existing files
    /// </summary>
    public bool Force { get; set; }

    /// <summary>
    /// The name of the ABP domain project
    /// </summary>
    public string DomainProjectName { get; set; }
    /// <summary>
    /// The name of the ABP EF Core project
    /// </summary>
    public string EfProjectName { get; set; }
    /// <summary>
    /// The name of the ABP MongoDB project
    /// </summary>
    public string MongodbProjectName { get; set; }
    /// <summary>
    /// The name of the ABP app contract project
    /// </summary>
    public string AppContractProjectName { get; set; }
    /// <summary>
    /// The name of the ABP app service project
    /// </summary>
    public string AppServiceProjectName { get; set; }
    /// <summary>
    /// The name of the ABP Http API project
    /// </summary>
    public string HttpApiProjectName { get; set; }

    //public bool NoTiered { get; set; }
}
