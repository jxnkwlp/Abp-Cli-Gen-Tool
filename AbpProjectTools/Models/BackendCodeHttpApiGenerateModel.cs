using System.Collections.Generic;

namespace AbpProjectTools.Models;

public class BackendCodeHttpApiGenerateModel
{
    public string SluName { get; set; }
    public ClassTypeInfoModel Type { get; set; }
    public string Name { get; set; }
    public string BaseControllerName { get; set; }

    public List<string> Namespaces { get; set; } = new List<string>();

    public Dictionary<string, BackendCodeHttpApiRouteGenerateModel> Routes { get; set; }
}
