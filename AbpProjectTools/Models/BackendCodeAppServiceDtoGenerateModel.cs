namespace AbpProjectTools.Models;

public class BackendCodeAppServiceDtoGenerateModel
{
    public string SluName { get; set; }
    public ClassTypeInfoModel Type { get; set; }

    public string OutputName { get; set; }

    public bool IsCreateOrUpdateType { get; set; }
    public bool IsEntityType { get; set; }
    public bool IsListRequestType { get; set; }
    public bool IsListResultType { get; set; }
}
