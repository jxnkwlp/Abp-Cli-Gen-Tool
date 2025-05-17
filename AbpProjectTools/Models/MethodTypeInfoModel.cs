using System.Collections.Generic;

namespace AbpProjectTools.Models;

public class MethodTypeInfoModel
{
    public string Name { get; set; }

    public string ReturnTypeCode { get; set; }
    public string ReturnTypeName { get; set; }

    public bool IsAsync { get; set; }

    public List<MethodMemberTypeInfoModel> Members { get; set; } = new List<MethodMemberTypeInfoModel>();

    public bool IsInherite { get; set; }
}
