using System.Collections.Generic;

namespace AbpProjectTools.Models
{
    public class ApiDefinition
    {
        public string OperationId { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string[] Tags { get; set; }

        public IList<ApiParamItem> PathParams { get; set; }
        public IList<ApiParamItem> QueryParams { get; set; }
        public IList<ApiParamItem> RequestParams { get; set; }
        public string RequestParamSchame { get; set; }
        public IList<ApiParamItem> ResponseParams { get; set; }
        public string ResponseParamSchame { get; set; }
    }
}
