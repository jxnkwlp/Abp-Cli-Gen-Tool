using System.Linq;
using System.Threading.Tasks;
using AbpProjectTools.Models;
using NSwag;

namespace AbpProjectTools
{
    public class SwaggerHelper
    {
        public async Task<SwaggerApiInfoModel> LoadAsync(string url)
        {
            var result = new SwaggerApiInfoModel();

            var document = await NSwag.OpenApiDocument.FromUrlAsync(url);

            foreach (var item in document.Operations)
            {
                var pathParams = item
                    .Operation
                    .ActualParameters
                    .Where(x => x.Kind == OpenApiParameterKind.Path)
                    .Select(x => new ApiItemParam { Name = x.Name, Type = (int)x.Schema.Type, Description = x.Description, Required = true });
                var queryParams = item
                    .Operation
                    .ActualParameters
                    .Where(x => x.Kind == OpenApiParameterKind.Query)
                    .Select(x => new ApiItemParam { Name = x.Name, Type = (int)x.Schema.Type, Description = x.Description, Required = x.IsRequired, Nullable = x.IsNullableRaw ?? false });

                var apiItem = new ApiItemDefinition()
                {
                    OperationId = item.Operation.OperationId,

                    Description = item.Operation.Description,
                    Method = item.Method,
                    Path = item.Path,
                    Tags = item.Operation.Tags.ToArray(),

                    PathParams = pathParams.ToList(),
                    QueryParams = queryParams.ToList(),

                };

                result.Apis.Add(apiItem);
            }

            return result;
        }
    }
}
