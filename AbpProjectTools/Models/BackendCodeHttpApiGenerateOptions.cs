namespace AbpProjectTools.Models;

public class BackendCodeHttpApiGenerateOptions : BackendCodeGenerateGlobalOptions
{
    public string[] Name { get; set; }
    public string BaseDir { get; set; }
    public bool Post { get; set; }
}
