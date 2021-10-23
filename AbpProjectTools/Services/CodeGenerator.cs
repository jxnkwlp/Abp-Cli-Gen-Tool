using System;
using System.IO;

namespace AbpProjectTools
{
    [Obsolete]
    public static class CodeGenerator
    {
        public static string GenerateDomainService(DomainEntityDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/DomainService.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    ;
        }

        public static string GenerateRepositoryInterface(DomainEntityDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/RepositoryInterface.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    ;
        }

        public static string GenerateEfCoreRepository(EfCoreContextDefinitions efCoreContext, DomainEntityDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/EfCoreRepository.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", efCoreContext.TypeNamespace)
                    .Replace("{{Name}}", efCoreContext.TypeName)
                    .Replace("{{DomainNameSpace}}", domain.TypeNamespace)
                    .Replace("{{DomainName}}", domain.TypeName)
                    .Replace("{{DomainKey}}", domain.TypeKey)
                    ;
        }

        public static string GenerateHttpApiController(DomainEntityDefinitions domain, string body)
        {
            var content = File.ReadAllText("./Tpl/HttpApiController.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    .Replace("{{Body}}", body)
                    ;
        }

        public static string GenerateAppServiceCrudInterface(DomainEntityDefinitions domain, string listRequestTypeName, string createTypeName, string updateTypeName)
        {
            var content = File.ReadAllText("./Tpl/AppServiceCrudInterface.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    .Replace("{{ListRequest}}", listRequestTypeName)
                    .Replace("{{Create}}", createTypeName)
                    .Replace("{{Update}}", updateTypeName)
                    ;
        }

        public static string GenerateAppServiceCrudService(DomainEntityDefinitions domain, string listRequestTypeName, string createTypeName, string updateTypeName)
        {
            var content = File.ReadAllText("./Tpl/AppServiceCrudImpl.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    .Replace("{{ListRequest}}", listRequestTypeName)
                    .Replace("{{Create}}", createTypeName)
                    .Replace("{{Update}}", updateTypeName)
                    ;
        }

        public static string GenerateAppServiceBasicInterface(DomainEntityDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/AppServiceCustomInterface.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    ;
        }

        public static string GenerateAppServiceBasicService(DomainEntityDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/AppServiceCustomImpl.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    ;
        }

        public static string GenerateAppServiceBasicDto(DomainEntityDefinitions domain, string name = null)
        {
            var content = File.ReadAllText("./Tpl/AppServiceBasicDto.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", name ?? domain.TypeName)

                    ;
        }

        public static string GenerateAppServiceEntityDto(DomainEntityDefinitions domain, string body, bool inherit = true, string name = null)
        {
            var content = inherit ? File.ReadAllText("./Tpl/AppServiceEntityDto.txt") : File.ReadAllText("./Tpl/AppServiceBasicDto.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", name ?? domain.TypeName)
                    .Replace("{{Body}}", body)
                    ;
        }

        public static string GenerateAppServiceListRequestDto(DomainEntityDefinitions domain, string name = null)
        {
            var content = File.ReadAllText("./Tpl/AppServiceListRequestDto.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespace)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", name ?? domain.TypeName)
                    ;
        }


    }
}
