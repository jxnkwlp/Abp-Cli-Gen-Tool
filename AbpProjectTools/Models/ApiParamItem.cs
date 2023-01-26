namespace AbpProjectTools.Models;

public class ApiParamItem
{
    public string Name { get; set; }
    public ApiParamType Type { get; set; }
    public string TypeLiteral { get; set; }
    public string Description { get; set; }
    public string Format { get; set; }
    public bool Required { get; set; } = true;
    public bool Nullable { get; set; } = false;
    public bool Enumerable { get; set; }
}
