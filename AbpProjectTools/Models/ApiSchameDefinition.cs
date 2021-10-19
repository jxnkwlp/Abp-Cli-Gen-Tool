using System.Collections.Generic;

namespace AbpProjectTools.Models
{
    public class ApiSchameDefinition
    {
        public string Name { get; set; }
        public IList<ApiParamItem> Params { get; set; }
    }
}
