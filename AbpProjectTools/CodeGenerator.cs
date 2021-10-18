using System;
using System.IO;

namespace AbpProjectTools
{
    [Obsolete]
    public static class CodeGenerator
    {
        public static string GenerateDomainService(DomainDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/DomainService.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    ;
        }

        public static string GenerateRepositoryInterface(DomainDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/RepositoryInterface.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    ;
        }

        public static string GenerateEfCoreRepository(EfCoreContextDefinitions efCoreContext, DomainDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/EfCoreRepository.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", efCoreContext.TypeNamespce)
                    .Replace("{{Name}}", efCoreContext.TypeName)
                    .Replace("{{DomainNameSpace}}", domain.TypeNamespce)
                    .Replace("{{DomainName}}", domain.TypeName)
                    .Replace("{{DomainKey}}", domain.TypeKey)
                    ;
        }

        public static string GenerateHttpApiController(DomainDefinitions domain, string body)
        {
            var content = File.ReadAllText("./Tpl/HttpApiController.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    .Replace("{{Body}}", body)
                    ;
        }

        public static string GenerateAppServiceCrudInterface(DomainDefinitions domain, string listRequestTypeName, string createTypeName, string updateTypeName)
        {
            var content = File.ReadAllText("./Tpl/AppServiceCrudInterface.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    .Replace("{{ListRequest}}", listRequestTypeName)
                    .Replace("{{Create}}", createTypeName)
                    .Replace("{{Update}}", updateTypeName)
                    ;
        }

        public static string GenerateAppServiceCrudService(DomainDefinitions domain, string listRequestTypeName, string createTypeName, string updateTypeName)
        {
            var content = File.ReadAllText("./Tpl/AppServiceCrudImpl.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    .Replace("{{ListRequest}}", listRequestTypeName)
                    .Replace("{{Create}}", createTypeName)
                    .Replace("{{Update}}", updateTypeName)
                    ;
        }

        public static string GenerateAppServiceBasicInterface(DomainDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/AppServiceCustomInterface.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    ;
        }

        public static string GenerateAppServiceBasicService(DomainDefinitions domain)
        {
            var content = File.ReadAllText("./Tpl/AppServiceCustomImpl.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", domain.TypeName)
                    ;
        }

        public static string GenerateAppServiceBasicDto(DomainDefinitions domain, string name = null)
        {
            var content = File.ReadAllText("./Tpl/AppServiceBasicDto.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", name ?? domain.TypeName)

                    ;
        }

        public static string GenerateAppServiceEntityDto(DomainDefinitions domain, string body, bool inherit = true, string name = null)
        {
            var content = inherit ? File.ReadAllText("./Tpl/AppServiceEntityDto.txt") : File.ReadAllText("./Tpl/AppServiceBasicDto.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", name ?? domain.TypeName)
                    .Replace("{{Body}}", body)
                    ;
        }

        public static string GenerateAppServiceListRequestDto(DomainDefinitions domain, string name = null)
        {
            var content = File.ReadAllText("./Tpl/AppServiceListRequestDto.txt");

            return content
                    .Replace("{{ProjectName}}", domain.ProjectName)
                    .Replace("{{Namespace}}", domain.TypeNamespce)
                    .Replace("{{Key}}", domain.TypeKey)
                    .Replace("{{Name}}", name ?? domain.TypeName)
                    ;
        }


    }
}
