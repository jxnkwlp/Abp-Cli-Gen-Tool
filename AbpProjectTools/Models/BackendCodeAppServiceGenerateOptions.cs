namespace AbpProjectTools.Models;

public class BackendCodeAppServiceGenerateOptions : BackendCodeGenerateGlobalOptions
{
    public string[] EntityName { get; set; }

    public bool Empty { get; set; }
    public bool Crud { get; set; }

    public bool SplitCreateUpdate { get; set; }
    public bool SplitResult { get; set; }

    //public string ListRequestTypeName { get; set; }
    //public string ListResultTypeName { get; set; }
    //public string CreateTypeName { get; set; }
    //public string UpdateTypeName { get; set; }

    public string BaseDir { get; set; }
}
