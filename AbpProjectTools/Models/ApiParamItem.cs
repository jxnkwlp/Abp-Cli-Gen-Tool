namespace AbpProjectTools.Models
{
    public class ApiParamItem
    {
        public string Name { get; set; }
        public ApiParamType Type { get; set; }
        public string TypeName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsNullable { get; set; }
        public string Description { get; set; }
    }
}
