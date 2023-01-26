using System.Collections.Generic;

namespace AbpProjectTools.Models;

public class ApiInfoModel
{
    public IList<ApiDefinition> Apis { get; set; }

    public IList<ApiSchameDefinition> Schames { get; set; }

    public ApiInfoModel()
    {
        Apis = new List<ApiDefinition>();
        Schames = new List<ApiSchameDefinition>();
    }
}
