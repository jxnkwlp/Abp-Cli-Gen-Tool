using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbpProjectTools.Models;
using NJsonSchema;
using NSwag;

namespace AbpProjectTools.Services;

public class OpenApiDocumentService
{
    private static readonly string[] _ignoreTypeNames = new string[] { "Assembly", "ConstructorInfo", "CustomAttributeData", "CustomAttributeNamedArgument", "CustomAttributeTypedArgument", "EventInfo", "FieldInfo", "ICustomAttributeProvider", "IntPtr", "MemberInfo", "MethodBase", "MethodInfo", "Module", "ModuleHandle", "ParameterInfo", "PropertyInfo", "RuntimeFieldHandle", "RuntimeMethodHandle", "RuntimeTypeHandle", "Type", "TypeInfo", "TypeAttribute", "StructLayoutAttribute" };

    public static bool UseFullTypeName { get; set; } = false;

    public async Task<ApiInfoModel> LoadAsync(string url, bool isYaml = false)
    {
        OpenApiDocument document;

        if (isYaml)
        {
            document = await OpenApiYamlDocument.FromUrlAsync(url);
        }
        else
        {
            document = await OpenApiDocument.FromUrlAsync(url);
        }

        document.GenerateOperationIds();

        if (!string.IsNullOrEmpty(document.OpenApi))
        {
            return LoadFromOpenApiDocument(document);
        }
        else
        {
            return LoadFromSwaggerDocument(document);
        }
    }

    private ApiInfoModel LoadFromOpenApiDocument(OpenApiDocument document)
    {
        // update title as schema name
        foreach (var item in document.Components.Schemas)
        {
            item.Value.Title = item.Key;
        }

        var result = new ApiInfoModel();

        // 
        foreach (var pathItem in document.Paths)
        {
            var methods = pathItem.Value;

            foreach (var method in methods)
            {
                var methodItem = method.Value;

                var pathParams = methodItem
                    .Parameters
                    .Where(x => x.Kind == OpenApiParameterKind.Path)
                    .Select(x => GetParamItem(x));
                var queryParams = methodItem
                    .Parameters
                    .Where(x => x.Kind == OpenApiParameterKind.Query)
                    .Select(x => GetParamItem(x));

                var apiDefinition = new ApiDefinition()
                {
                    OperationId = methodItem.OperationId,
                    Description = methodItem.Description,
                    Method = method.Key,
                    Path = pathItem.Key,
                    Tags = methodItem.Tags.ToArray(),
                    PathParams = pathParams.ToList(),
                    QueryParams = queryParams.ToList(),
                };

                if (methodItem.RequestBody != null)
                {
                    JsonSchema requestBodySchema = null;
                    if (methodItem.RequestBody.Content.ContainsKey("application/json"))
                    {
                        requestBodySchema = methodItem.RequestBody.Content["application/json"].Schema;
                        apiDefinition.RequestType = "json";
                    }
                    else if (methodItem.RequestBody.Content.ContainsKey("multipart/form-data"))
                    {
                        requestBodySchema = methodItem.RequestBody.Content["multipart/form-data"].Schema;
                        apiDefinition.RequestType = "form";
                    }
                    else
                    {
                        requestBodySchema = methodItem.RequestBody.Content.First().Value.Schema;
                    }

                    if (requestBodySchema.HasReference)
                    {
                        apiDefinition.RequestParamSchame = FormatTypeName(requestBodySchema.Reference.Title);
                    }
                    else if (string.IsNullOrWhiteSpace(requestBodySchema.Title))
                    {
                        apiDefinition.RequestParamSchame = $"{apiDefinition.OperationId}Request";

                        // create schame
                        var props = new List<ApiParamItem>();
                        LoopActualProperties(requestBodySchema.ActualProperties, props);
                        result.Schames.Add(new ApiSchameDefinition()
                        {
                            Name = $"{apiDefinition.OperationId}Request",
                            Params = props,
                        });
                    }
                    else
                    {
                        apiDefinition.RequestParamSchame = FormatTypeName(requestBodySchema.Title);
                    }
                }

                if (methodItem.ActualResponses.ContainsKey("200"))
                {
                    var schema = methodItem.ActualResponses["200"].Schema;
                    if (schema?.HasReference == true)
                    {
                        apiDefinition.ResponseParamSchame = FormatTypeName(schema.Reference.Title);
                    }
                    else
                    {
                        // TODO 
                    }
                }

                //
                foreach (var response in methodItem.ActualResponses)
                {
                    apiDefinition.Description += $"\n *{response.Key}* {response.Value.Description}";
                }

                result.Apis.Add(apiDefinition);
            }
        }

        foreach (var item in document.Components.Schemas)
        {
            var schemaItem = item.Value;

            var typeName = item.Key;

            var props = new List<ApiParamItem>();

            if (_ignoreTypeNames.Any(x => x.EndsWith("." + typeName)) || typeName.StartsWith("System."))
                continue;

            var s = new ApiSchameDefinition()
            {
                Name = UseFullTypeName ? typeName.Replace(".", null) : GetShortTypeName(typeName),
                Params = props,
            };

            if (item.Value.IsEnumeration)
            {
                s.Enumerable = true;
                s.EnumValues = item.Value.Enumeration?.ToList();
                s.EnumNames = item.Value.EnumerationNames?.ToList();
                s.EnumValueAsInter = item.Value.Type == JsonObjectType.Integer;
            }
            else
            {
                LoopActualProperties(schemaItem.ActualProperties, props);
            }

            result.Schames.Add(s);
        }

        // check enum types
        var allEmunTypes = result.Schames.Where(x => x.Enumerable);
        foreach (var schame in result.Schames)
        {
            if (!schame.Enumerable && schame.Params != null)
            {
                foreach (var @param in schame.Params)
                {
                    if (allEmunTypes.Any(x => x.Name == @param.TypeLiteral))
                    {
                        param.Enumerable = true;
                    }
                }
            }
        }

        return result;
    }

    private ApiInfoModel LoadFromSwaggerDocument(OpenApiDocument document)
    {
        // update title as schema name
        foreach (var item in document.Definitions)
        {
            var typeName = item.Key;
            if (item.Key.Contains("."))
                typeName = item.Key.Substring(item.Key.LastIndexOf('.') + 1);

            item.Value.Title = typeName;
        }

        var result = new ApiInfoModel();

        foreach (var item in document.Operations)
        {
            var pathParams = item
                .Operation
                .ActualParameters
                .Where(x => x.Kind == OpenApiParameterKind.Path)
                .Select(x => GetParamItem(x));
            var queryParams = item
                .Operation
                .ActualParameters
                .Where(x => x.Kind == OpenApiParameterKind.Query)
                .Select(x => GetParamItem(x));

            var apiItem = new ApiDefinition()
            {
                OperationId = item.Operation.OperationId,

                Description = item.Operation.Description,
                Method = item.Method,
                Path = item.Path,
                Tags = item.Operation.Tags.ToArray(),

                PathParams = pathParams.ToList(),
                QueryParams = queryParams.ToList(),

            };


            if (item.Operation.RequestBody != null)
            {
                JsonSchema requestBodySchema = null;
                if (item.Operation.RequestBody.Content.ContainsKey("application/json"))
                {
                    requestBodySchema = item.Operation.RequestBody.Content["application/json"].Schema;
                }
                else
                {
                    requestBodySchema = item.Operation.RequestBody.Content.First().Value.Schema;
                }

                if (requestBodySchema.HasReference)
                {
                    apiItem.RequestParamSchame = requestBodySchema.Reference.Title;
                }
                else
                {
                    // TODO 
                }
            }

            if (item.Operation.ActualResponses.ContainsKey("200"))
            {
                var schema = item.Operation.ActualResponses["200"].Schema;
                if (schema?.HasReference == true)
                {
                    apiItem.ResponseParamSchame = schema.Reference.Title;
                }
                else
                {
                    // TODO 
                }
            }


            result.Apis.Add(apiItem);
        }


        foreach (var item in document.Definitions)
        {
            var typeName = item.Value.Title;
            var props = new List<ApiParamItem>();

            if (_ignoreTypeNames.Contains(typeName) || typeName.StartsWith("System."))
                continue;

            var s = new ApiSchameDefinition()
            {
                Name = typeName.Replace(".", null),
                Params = props,
            };

            if (item.Value.IsEnumeration)
            {
                s.Enumerable = true;
                s.EnumValues = item.Value.Enumeration?.ToList();
                s.EnumNames = item.Value.EnumerationNames?.ToList();
                s.EnumValueAsInter = item.Value.Type == JsonObjectType.Integer;
            }
            else
            {
                LoopActualProperties(item.Value.ActualProperties, props);
            }

            result.Schames.Add(s);
        }

        // check eunm types
        var allEmunTypes = result.Schames.Where(x => x.Enumerable);
        foreach (var schame in result.Schames)
        {
            if (!schame.Enumerable && schame.Params != null)
            {
                foreach (var @param in schame.Params)
                {
                    if (allEmunTypes.Any(x => x.Name == @param.TypeLiteral))
                    {
                        param.Enumerable = true;
                    }
                }
            }
        }

        return result;
    }

    private static ApiParamType GetItemType(JsonObjectType type, string format = null)
    {
        if (format == "binary")
            return ApiParamType.Unknow;

        switch (type)
        {
            case JsonObjectType.Boolean: return ApiParamType.Boolean;
            case JsonObjectType.Number: return ApiParamType.Number;
            case JsonObjectType.Integer: return ApiParamType.Number;
            case JsonObjectType.String: return ApiParamType.String;
            case JsonObjectType.Object: return ApiParamType.Object;
            case JsonObjectType.Array: return ApiParamType.Array;

            default: return ApiParamType.Unknow;
        }
    }

    private static void LoopActualProperties(IReadOnlyDictionary<string, JsonSchemaProperty> actualProperties, List<ApiParamItem> result)
    {
        foreach (var prop in actualProperties)
        {
            var p = new ApiParamItem()
            {
                Name = ConvertName(prop.Key),
                Type = GetItemType(prop.Value.Type, prop.Value.Format),
                TypeLiteral = "", //
                Description = prop.Value.Description,
                Required = prop.Value.IsRequired, //.IsNullableRaw == true ? false : true,
                Nullable = !prop.Value.IsRequired,
                MaxLength = prop.Value.MaxLength,
                Format = prop.Value.Format,
            };

            if (!prop.Value.IsRequired && prop.Value.IsNullableRaw.HasValue != true)
            {
                p.Required = true;
                p.Nullable = false;
            }

            if (prop.Value.HasReference && prop.Value.Reference != null)
            {
                p.Type = ApiParamType.Object;
                p.TypeLiteral = FormatTypeName(prop.Value.Reference.Title ?? "any");
                p.ReferenceObjectName = prop.Value.Reference.Title;
            }
            else
            {
                if (p.Type == ApiParamType.Array)
                {
                    var arrayItem = prop.Value.Item;
                    if (arrayItem.HasReference)
                    {
                        p.TypeLiteral = FormatTypeName(arrayItem.Reference.Title);
                        p.ReferenceObjectName = p.TypeLiteral;
                    }
                    else if (arrayItem.Type == JsonObjectType.None)
                    {
                        p.TypeLiteral = FormatTypeName(arrayItem.Title ?? "any");
                    }
                    else
                    {
                        var arrayItemType = GetItemType(arrayItem.Type);
                        p.TypeLiteral = arrayItemType.ToString().ToLowerInvariant();
                    }
                }
                else if (p.Type == ApiParamType.Object && prop.Value.AdditionalPropertiesSchema?.Reference != null)
                {
                    if (prop.Value.AdditionalPropertiesSchema != null && prop.Value.AdditionalPropertiesSchema.Reference != null)
                    {
                        p.Type = ApiParamType.CompositeObject;
                        p.TypeLiteral = FormatTypeName(prop.Value.AdditionalPropertiesSchema.Reference.Title);
                        p.ReferenceObjectName = p.TypeLiteral;
                    }
                }
                else if (prop.Value.ActualTypeSchema != null && prop.Value.HasOneOfSchemaReference)
                {
                    p.Type = ApiParamType.Object;
                    p.TypeLiteral = FormatTypeName(prop.Value.ActualTypeSchema.Title);
                    p.ReferenceObjectName = p.TypeLiteral;
                }
                else if (p.Type == ApiParamType.Object && string.IsNullOrEmpty(p.TypeLiteral))
                {
                    p.TypeLiteral = "any";
                }
                else
                {
                    // special type
                    if (p.TypeLiteral == "JObject")
                    {
                        p.TypeLiteral = "any";
                    }
                    else if (p.TypeLiteral == "JArray")
                    {
                        p.TypeLiteral = "any[]";
                    }
                }
            }

            // special type
            if (_ignoreTypeNames.Contains(p.TypeLiteral) || p.TypeLiteral?.StartsWith("System.") == true)
            {
                p.TypeLiteral = "any";
            }

            // can be null
            if (p.Type == ApiParamType.Boolean)
            {
                p.Nullable = true;
                p.Required = false;
            }

            if (p.TypeLiteral == "any")
            {
                p.Nullable = true;
                p.Required = false;
            }

            result.Add(p);
        }
    }

    private static ApiParamItem GetParamItem(OpenApiParameter item)
    {
        var itemType = GetItemType(item.Schema.Type);
        string typeName = null;

        if (itemType == ApiParamType.Array)
        {
            var arrayItem = item.ActualTypeSchema.Item;
            if (arrayItem.HasReference)
            {
                typeName = FormatTypeName(arrayItem.Reference.Title);
            }
            else if (arrayItem.Type == JsonObjectType.None)
            {
                typeName = FormatTypeName(arrayItem.Title ?? "any");
            }
            else if (arrayItem.Type == JsonObjectType.Object)
            {
                typeName = "any";
            }
            else
            {
                var arrayItemType = GetItemType(arrayItem.Type);
                typeName = arrayItemType.ToString().ToLowerInvariant();
            }
        }
        else if (itemType == ApiParamType.Object)
        {
            typeName = "any";
        }
        else
        {
            typeName = itemType.ToString().ToLowerInvariant();
        }

        return new ApiParamItem
        {
            Name = item.Name,
            Type = itemType,
            TypeLiteral = typeName,
            Description = item.Description,
            Required = item.IsRequired,
            Nullable = !item.IsRequired,
            Format = item.Format,
        };
    }

    private static string FormatTypeName(string typeName)
    {
        return UseFullTypeName ? typeName.Replace(".", null).Replace("+", null) : GetShortTypeName(typeName);
    }

    private static string GetShortTypeName(string fullName)
    {
        var typeName = fullName;
        if (fullName.Contains('.'))
            typeName = fullName.Substring(fullName.LastIndexOf('.') + 1);
        return typeName.Replace("+", null);
    }

    private static string ConvertName(string name)
    {
        return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(name);
    }
}
