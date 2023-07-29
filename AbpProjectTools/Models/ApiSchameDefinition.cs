using System;
using System.Collections.Generic;

namespace AbpProjectTools.Models;

public class ApiSchameDefinition : IEquatable<ApiSchameDefinition>
{
    public string Name { get; set; }

    public bool Enumerable { get; set; }

    public List<object> EnumValues { get; set; }

    public List<string> EnumNames { get; set; }

    public bool EnumValueAsInter { get; set; }

    public IList<ApiParamItem> Params { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as ApiSchameDefinition);
    }

    public bool Equals(ApiSchameDefinition other)
    {
        return other is not null &&
               Name == other.Name &&
               Enumerable == other.Enumerable;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Enumerable);
    }

    public static bool operator ==(ApiSchameDefinition left, ApiSchameDefinition right)
    {
        return EqualityComparer<ApiSchameDefinition>.Default.Equals(left, right);
    }

    public static bool operator !=(ApiSchameDefinition left, ApiSchameDefinition right)
    {
        return !(left == right);
    }
}
