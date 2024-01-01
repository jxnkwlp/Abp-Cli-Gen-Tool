using System;
using System.Collections.Generic;

namespace AbpProjectTools.Models;

public class ApiParamItem : IEquatable<ApiParamItem>
{
    public string Name { get; set; }
    public ApiParamType Type { get; set; }
    public string TypeLiteral { get; set; }
    public string Description { get; set; }
    public string Format { get; set; }
    public bool Required { get; set; } = true;
    public bool Nullable { get; set; } = false;
    public int? MaxLength { get; set; }
    public bool Enumerable { get; set; }

    public string ReferenceObjectName { get; set; }

    public ApiSchameDefinition ObjectDefinition { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as ApiParamItem);
    }

    public bool Equals(ApiParamItem other)
    {
        return other is not null &&
               Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }

    public override string ToString()
    {
        return $"[{Type}] {Name}";
    }

    public static bool operator ==(ApiParamItem left, ApiParamItem right)
    {
        return EqualityComparer<ApiParamItem>.Default.Equals(left, right);
    }

    public static bool operator !=(ApiParamItem left, ApiParamItem right)
    {
        return !(left == right);
    }
}
