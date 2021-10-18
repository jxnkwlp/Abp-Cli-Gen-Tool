using System.Collections.Generic;

namespace AbpProjectTools.Models
{
    public class ApiItemDefinition
    {
        public string OperationId { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string[] Tags { get; set; }

        public IList<ApiItemParam> PathParams { get; set; }
        public IList<ApiItemParam> QueryParams { get; set; }
        public IList<ApiItemParam> RequestParams { get; set; }
        public IList<ApiItemParam> ResponseParams { get; set; }

    }

    public class ApiItemParam
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public bool Required { get; set; }
        public bool Nullable { get; set; }
        public string Description { get; set; }
    }

    public class ApiSchameItemDefinition { }
}
