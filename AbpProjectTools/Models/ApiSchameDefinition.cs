using System.Collections.Generic;

namespace AbpProjectTools.Models
{
    public class ApiSchameDefinition
    {
        public string Name { get; set; }

        public bool Enumerable { get; set; }
        public List<object> EnumValues { get; set; }

        public IList<ApiParamItem> Params { get; set; }
    }
}
