using System.Collections.Generic;

namespace AbpProjectTools.Models
{
    public class GenerateTypeScriptServicesCodeData
    {
        public IList<ApiItemDefinition> Apis { get; set; }
        public int Count { get; set; }
        public string Url { get; set; }
    }
}
