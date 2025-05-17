namespace AbpProjectTools.Models;

public class BackendCodeDomainGenerateModel
{
    public string SluName { get; set; }

    public ClassTypeInfoModel Type { get; set; }

    public bool ReadonlyRepo { get; set; }
}
