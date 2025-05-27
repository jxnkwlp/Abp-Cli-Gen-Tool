using ICSharpCode.Decompiler.TypeSystem;

namespace AbpProjectTools.Models;

public class MemberTypeInfoModel
{
    public string MemberName { get; set; }

    public string TypeName { get; set; }
    public string TypeCode { get; set; }
    public string TypeFullName { get; set; }
    public string TypeNamespace { get; set; }

    public bool CanSet { get; set; }
    public bool CanGet { get; set; }

    public bool IsNullable { get; set; }
    public bool IsClass { get; set; }
    public bool IsInherit { get; set; }

    public IModule TypeModule { get; set; }

    public override string ToString()
    {
        return $"{TypeFullName} {MemberName}";
    }

}
