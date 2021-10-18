using System.Collections.Generic;
using AbpProjectTools.Models;

namespace AbpProjectTools
{
    public class SwaggerApiInfoModel
    {
        public IList<ApiItemDefinition> Apis { get; set; }

        public IList<ApiSchameItemDefinition> ApiSchames { get; set; }

        public SwaggerApiInfoModel()
        {
            Apis = new List<ApiItemDefinition>();
            ApiSchames = new List<ApiSchameItemDefinition>();
        }
    }
}
