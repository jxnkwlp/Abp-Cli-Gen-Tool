namespace AbpProjectTools.Models;

public class BackendCodeRepositoryGenerateOptions : BackendCodeGenerateGlobalOptions
{
    /// <summary>
    ///  The entity(s) name
    /// </summary>
    public string[] EntityName { get; set; }

    public bool Ef { get; set; } = true;
    public bool Mongodb { get; set; } = true;

    public bool ReadonlyRepo { get; set; }

    public bool Interface { get; set; } = true;
}
