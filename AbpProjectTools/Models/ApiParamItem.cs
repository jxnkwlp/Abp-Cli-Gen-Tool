namespace AbpProjectTools.Models
{
    public class ApiParamItem
    {
        public string Name { get; set; }
        public ApiParamType Type { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public bool Required { get; set; }
        public bool Nullable { get; set; }
        public bool Enumerable { get; set; }
    }
}
