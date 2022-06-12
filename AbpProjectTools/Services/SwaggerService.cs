using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbpProjectTools.Models;
using NJsonSchema;
using NSwag;

namespace AbpProjectTools.Services
{
    public class SwaggerService
    {
        public async Task<SwaggerApiInfoModel> LoadAsync(string url)
        {
            var result = new SwaggerApiInfoModel();

            var document = await OpenApiDocument.FromUrlAsync(url);

            document.GenerateOperationIds();

            foreach (var item in document.Definitions)
            {
                var typeName = item.Key;
                if (item.Key.Contains("."))
                    typeName = item.Key.Substring(item.Key.LastIndexOf('.') + 1);

                item.Value.Title = typeName;
            }

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

                var s = new ApiSchameDefinition()
                {
                    Name = typeName,
                    Params = props,
                };

                if (item.Value.IsEnumeration)
                {
                    s.Enumerable = true;
                    s.EnumValues = item.Value.Enumeration?.ToList();
                    s.EnumNames = item.Value.EnumerationNames?.ToList();
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
                        if (allEmunTypes.Any(x => x.Name == @param.TypeName))
                        {
                            param.Enumerable = true;
                        }
                    }
                }
            }

            return result;
        }

        private static ApiParamType GetItemType(JsonObjectType type)
        {
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
                    Name = prop.Key,
                    Type = GetItemType(prop.Value.Type),
                    // TypeName = prop.Value.HasReference ? prop.Value.Reference.Title : (prop.Value.Type == JsonObjectType.Array ? GetItemType(prop.Value.Item.Type).ToString() : null),
                    Description = prop.Value.Description,
                    Required = prop.Value.IsRequired, //.IsNullableRaw == true ? false : true,
                    Nullable = !prop.Value.IsRequired,
                    Format = prop.Value.Format,
                };

                if (prop.Value.HasReference)
                {
                    p.Type = ApiParamType.Object;
                    p.TypeName = prop.Value.Reference.Title;
                }
                else
                {

                    //if (prop.Value.IsEnumeration)
                    //{
                    //    p.Enumerable = true;
                    //    p.EnumValues = prop.Value.Enumeration?.ToList();
                    //    p.EnumNames = prop.Value.EnumerationNames?.ToList();
                    //}

                    if (p.Type == ApiParamType.Array)
                    {
                        var arrayItem = prop.Value.Item;
                        if (arrayItem.HasReference)
                        {
                            p.TypeName = arrayItem.Reference.Title;
                        }
                        else if (arrayItem.Type == JsonObjectType.None)
                        {
                            p.TypeName = arrayItem.Title;
                        }
                        else
                        {
                            var arrayItemType = GetItemType(arrayItem.Type);
                            p.TypeName = arrayItemType.ToString().ToLowerInvariant();
                        }
                    }
                    else if (p.Type == ApiParamType.Object)
                    {
                        // p.TypeName = "any"; 
                        if (prop.Value.AdditionalPropertiesSchema != null && prop.Value.AdditionalPropertiesSchema.Reference != null)
                        {
                            p.Type = ApiParamType.CompositeObject;
                            p.TypeName = prop.Value.AdditionalPropertiesSchema.Reference.Title;
                        }
                    }
                    else
                    {
                        p.TypeName = p.Type.ToString().ToLowerInvariant();
                    }
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
                    typeName = arrayItem.Reference.Title;
                }
                else if (arrayItem.Type == JsonObjectType.None)
                {
                    typeName = arrayItem.Title;
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
                TypeName = typeName,
                Description = item.Description,
                Required = item.IsRequired,
                Nullable = !item.IsRequired,
                Format = item.Format,
            };
        }
    }
}
