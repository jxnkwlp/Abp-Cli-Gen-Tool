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
                    .Select(x => new ApiParamItem
                    {
                        Name = x.Name,
                        Type = GetItemType(x.Schema.Type),
                        Description = x.Description,
                        Required = x.IsRequired,
                        Nullable = !x.IsRequired,
                    });
                var queryParams = item
                    .Operation
                    .ActualParameters
                    .Where(x => x.Kind == OpenApiParameterKind.Query)
                    .Select(x => new ApiParamItem
                    {
                        Name = x.Name,
                        Type = GetItemType(x.Schema.Type),
                        Description = x.Description,
                        Required = x.IsRequired,
                        Nullable = !x.IsRequired,
                    });

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
                }
                else
                {
                    foreach (var prop in item.Value.ActualProperties)
                    {
                        var p = new ApiParamItem()
                        {
                            Name = prop.Key,
                            Type = GetItemType(prop.Value.Type),
                            // TypeName = prop.Value.HasReference ? prop.Value.Reference.Title : (prop.Value.Type == JsonObjectType.Array ? GetItemType(prop.Value.Item.Type).ToString() : null),
                            Description = prop.Value.Description,
                            Required = prop.Value.IsRequired,
                        };

                        if (prop.Value.HasReference)
                        {
                            p.Type = ApiParamType.Object;
                            p.TypeName = prop.Value.Reference.Title;
                        }
                        else
                        {
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
                            else
                            {
                                p.TypeName = p.Type.ToString().ToLowerInvariant();
                            }
                        }

                        props.Add(p);
                    }
                }

                result.Schames.Add(s);
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
    }
}
