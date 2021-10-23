using System.Collections.Generic;
using AbpProjectTools.Models;

namespace AbpProjectTools
{
    public class SwaggerApiInfoModel
    {
        public IList<ApiDefinition> Apis { get; set; }

        public IList<ApiSchameDefinition> Schames { get; set; }

        public SwaggerApiInfoModel()
        {
            Apis = new List<ApiDefinition>();
            Schames = new List<ApiSchameDefinition>();
        }
    }
}
