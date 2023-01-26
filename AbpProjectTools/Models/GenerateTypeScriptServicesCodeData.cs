using System.Collections.Generic;

namespace AbpProjectTools.Models;

public class GenerateTypeScriptServicesCodeData
{
    public IList<ApiDefinition> Apis { get; set; }
    public int Count { get; set; }
    public string Url { get; set; }
}
