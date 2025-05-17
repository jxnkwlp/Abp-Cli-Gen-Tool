namespace AbpProjectTools.Models;

public class BackendCodeAppServiceGenerateModel
{
    public string SluName { get; set; }
    public ClassTypeInfoModel Type { get; set; }

    public bool Empty { get; set; }
    public bool Crud { get; set; }

    public string ListRequestTypeName { get; set; }
    public string ListResultTypeName { get; set; }
    public string CreateTypeName { get; set; }
    public string UpdateTypeName { get; set; }
}
