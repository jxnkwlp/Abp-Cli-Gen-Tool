using System.Collections.Generic;
using AbpProjectTools.Models;

namespace AbpProjectTools
{
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
}
