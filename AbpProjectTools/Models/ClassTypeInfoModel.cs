using System.Collections.Generic;

namespace AbpProjectTools.Models;

public class ClassTypeInfoModel
{
    public string Key { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public string Namespace { get; set; }
    public string BaseType { get; set; }
    public string BaseAbpType { get; set; }
    public bool IsAggregatedType { get; set; }
    public bool IsAbpType { get; set; }

    public List<string> ReferencesImports { get; set; }

    public List<MemberTypeInfoModel> Members { get; set; } = new List<MemberTypeInfoModel>();

    public List<MethodTypeInfoModel> Methods { get; set; } = new List<MethodTypeInfoModel>();

    public List<ClassTypeInfoModel> References { get; set; } = new List<ClassTypeInfoModel>();

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Key))
            return FullName;
        return $"{FullName}<{Key}>";
    }
}
