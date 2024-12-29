using AbpProjectTools.Commands;
using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace AbpProjectTools.Services;

public class BackendCodeCommandExecutor
{
    private bool _displayed = false;
    private readonly string _solutionDir;

    public BackendCodeCommandExecutor(string solutionDir)
    {
        _solutionDir = solutionDir;
    }

    private void Display(BackendCodeGeneratorCommonCommandOption options)
    {
        if (_displayed)
            return;
        _displayed = true;

        Console.WriteLine($"🚩 Project directory: {Directory.GetCurrentDirectory().PastelBg(Color.SlateGray)}");
        Console.WriteLine($"🍕 Project name: {options.ProjectName.Pastel(Color.GreenYellow)}");
        Console.WriteLine($"🍴 Entity: {options.Name.Pastel(Color.GreenYellow)}");
    }

    private void Check(BackendCodeGeneratorCommonCommandOption options)
    {
        if (string.IsNullOrWhiteSpace(options.ProjectName))
            options.ProjectName = GetSolutionName(_solutionDir);

        if (string.IsNullOrEmpty(options.ProjectName))
            throw new Exception("The project can't be empty.");
    }

    public void GenerateAllCode(BackendAppServiceCodeGeneratorCommandOption options)
    {
        Check(options);
        Display(options);

        if (!SourceBuildHelper.Build(_solutionDir))
        {
            return;
        }

        // domain service
        GenerateDomainServiceCode(options);

        if (!SourceBuildHelper.Build(_solutionDir))
        {
            return;
        }

        // repository
        GenerateRepositoryCode(options);

        if (!SourceBuildHelper.Build(_solutionDir))
        {
            return;
        }

        // app service
        GenerateAppServiceCode(options);

        if (!SourceBuildHelper.Build(_solutionDir))
        {
            return;
        }

        // http controller
        GenerateHttpControllerCode(options);
    }

    public void GenerateDomainServiceCode(BackendCodeGeneratorCommonCommandOption options)
    {
        Check(options);
        Display(options);

        var typeService = new TypeService(options.SluDir);

        var templateService = new TemplateService(options.Template);

        try
        {
            Console.WriteLine($"🚗 Staring generate domain '{options.Name}' service code ...");

            var domainInfo = typeService.GetDomain(options.Name);

            var fileContent = templateService.Render("DomainService", domainInfo);

            var filePath = Path.Combine(domainInfo.FileDirectory, $"{domainInfo.TypeName}Manager.cs");

            WriteFileContent(filePath, fileContent, options.Overwrite);

            Console.WriteLine("🎉🎉🎉 Done ".Pastel(Color.Green));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.Pastel(Color.Red));
            Console.WriteLine(ex.ToString().Pastel(Color.SlateGray));
        }
    }

    public void GenerateRepositoryCode(BackendCodeGeneratorCommonCommandOption options)
    {
        Check(options);
        Display(options);

        var typeService = new TypeService(options.SluDir);
        var templateService = new TemplateService(options.Template);

        try
        {
            Console.WriteLine($"🚗 Staring generate domain '{options.Name}' repository code ..");

            var domainInfo = typeService.GetDomain(options.Name);
            var efInfo = typeService.GetEfCore(false);
            var mongdbInfo = typeService.GetMongoDB(false);

            if (efInfo == null && mongdbInfo == null)
            {
                throw new Exception("EntityFrameworkCore and MongoDB not found. Please check again or build project first.");
            }

            // file 1
            var fileContent = templateService.Render("DomainRepository", domainInfo);
            var filePath = Path.Combine(domainInfo.FileDirectory, $"I{domainInfo.TypeName}Repository.cs");

            WriteFileContent(filePath, fileContent, options.Overwrite);

            // file 2 -- EfCore
            if (efInfo != null && Directory.Exists(efInfo.FileDirectoryName))
            {
                fileContent = templateService.Render("EfCoreRepository", new { domain = domainInfo, info = efInfo });
                filePath = Path.Combine(efInfo.FileDirectoryName, "Repositories", $"{domainInfo.TypeName}Repository.cs");

                WriteFileContent(filePath, fileContent, options.Overwrite);
            }

            // file 3 -- MongoDb
            if (mongdbInfo != null && Directory.Exists(mongdbInfo.FileDirectoryName))
            {
                fileContent = templateService.Render("MongoDBRepository", new { domain = domainInfo, info = mongdbInfo });
                filePath = Path.Combine(mongdbInfo.FileDirectoryName, "Repositories", $"{domainInfo.TypeName}Repository.cs");

                WriteFileContent(filePath, fileContent, options.Overwrite);
            }

            // file 4 - DomainService
            fileContent = templateService.Render("DomainService", domainInfo);

            filePath = Path.Combine(domainInfo.FileDirectory, $"{domainInfo.TypeName}Manager.cs");

            WriteFileContent(filePath, fileContent, false);

            Console.WriteLine("🎉🎉🎉 Done ".Pastel(Color.Green));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.Pastel(Color.Red));
            Console.WriteLine(ex.ToString().Pastel(Color.SlateGray));
        }
    }

    public void GenerateAppServiceCode(BackendAppServiceCodeGeneratorCommandOption options)
    {
        Check(options);
        Display(options);

        if (string.IsNullOrEmpty(options.ListRequestTypeName))
            options.ListRequestTypeName = $"{options.Name}ListRequestDto";

        if (string.IsNullOrEmpty(options.CreateTypeName))
            options.CreateTypeName = $"{options.Name}CreateDto";

        if (string.IsNullOrEmpty(options.UpdateTypeName))
            options.UpdateTypeName = $"{options.Name}UpdateDto";

        if (string.IsNullOrEmpty(options.ListResultTypeName))
            options.ListResultTypeName = $"{options.Name}Dto";

        if (!options.SplitCuType)
        {
            options.CreateTypeName = $"{options.Name}CreateOrUpdateDto";
            options.UpdateTypeName = $"{options.Name}CreateOrUpdateDto";
        }

        if (options.SplitListResultType)
        {
            options.ListResultTypeName = $"{options.Name}BasicDto";
        }

        var typeService = new TypeService(options.SluDir);

        var templateService = new TemplateService(options.Template);

        var ContractsProject = FileHelper.GetApplicationContractProjectDirectory(options.SluDir);
        var appServiceProject = FileHelper.GetApplicationProjectDirectory(options.SluDir);

        try
        {
            Console.WriteLine($"🚗 Staring generate '{options.Name}' app service code ...");

            var domainInfo = typeService.GetDomain(options.Name, true);
            var appTypeDefinitions = typeService.GetAppContractTypeDefinitions();

            // Application Contracts
            var fileContent = templateService.Render("AppServiceInterface", new
            {
                // serviceName = appServiceInfo.ServiceName,
                projectName = options.ProjectName,
                domain = domainInfo,
                CreateTypeName = options.CreateTypeName,
                UpdateTypeName = options.UpdateTypeName,
                ListRequestTypeName = options.ListRequestTypeName,
                ListResultTypeName = options.ListResultTypeName,
                SplitCuType = options.SplitCuType,
                SplitListType = options.SplitListResultType,
                BasicService = options.BasicService,
                Crud = options.Crud,
            });

            var filePath = Path.Combine(ContractsProject.FullName, domainInfo.FileProjectPath, $"I{domainInfo.TypeName}AppService.cs");

            WriteFileContent(filePath, fileContent, options.Overwrite);

            // Application Services
            fileContent = templateService.Render("AppServiceService", new
            {
                projectName = options.ProjectName,
                domain = domainInfo,
                CreateTypeName = options.CreateTypeName,
                UpdateTypeName = options.UpdateTypeName,
                ListRequestTypeName = options.ListRequestTypeName,
                ListResultTypeName = options.ListResultTypeName,
                SplitCuType = options.SplitCuType,
                SplitListType = options.SplitListResultType,
                BasicService = options.BasicService,
                Crud = options.Crud,
            });

            filePath = Path.Combine(appServiceProject.FullName, domainInfo.FileProjectPath, $"{domainInfo.TypeName}AppService.cs");

            WriteFileContent(filePath, fileContent, options.Overwrite);

            // dto
            void GenerateDto(string typeName, EntityDefinitions entityDefinitions, bool isListRequestType = false, bool isCreateType = false, bool isUpdateType = false, bool isEntityType = false, bool isListResultType = false)
            {
                fileContent = templateService.Render("AppServiceDto", new
                {
                    ProjectName = options.ProjectName,
                    Domain = entityDefinitions,
                    TypeName = typeName,
                    IsCreateType = isCreateType,
                    IsUpdateType = isUpdateType,
                    IsListRequestType = isListRequestType,
                    IsEntityType = isEntityType,
                    IsListResultType = isListResultType,
                });

                filePath = Path.Combine(ContractsProject.FullName, domainInfo.FileProjectPath, $"{typeName}.cs");

                WriteFileContent(filePath, fileContent, options.Overwrite);
            }

            foreach (var item in domainInfo.Properties)
            {
                if (item.IsClass)
                {
                    var dtoType = appTypeDefinitions.DtoTypes.FirstOrDefault(x => x.Name == item.Name + "Dto");
                    if (dtoType != null)
                    {
                        item.Type = dtoType.Name;
                    }
                    else
                    {
                        // generate
                        var referenceTypeDefinition = typeService.GetDomain(item.InnerTypeFullName ?? item.Type, true);

                        GenerateDto($"{referenceTypeDefinition.TypeName}Dto", referenceTypeDefinition, isEntityType: !string.IsNullOrEmpty(referenceTypeDefinition.TypeKey));

                        item.TypeCode = item.TypeCode.Replace(item.InnerType, $"{item.InnerType}Dto");
                    }
                }
            }

            if (options.BasicService == false)
            {
                GenerateDto(options.ListRequestTypeName, domainInfo, isListRequestType: true);

                if (options.SplitCuType)
                {
                    GenerateDto(options.CreateTypeName, domainInfo, isCreateType: true);
                    GenerateDto(options.UpdateTypeName, domainInfo, isUpdateType: true);
                }
                else
                {
                    GenerateDto(options.CreateTypeName, domainInfo, isCreateType: true);
                }

                if (options.SplitListResultType)
                {
                    GenerateDto($"{domainInfo.TypeName}BasicDto", domainInfo, isListResultType: true);
                }

                GenerateDto($"{domainInfo.TypeName}Dto", domainInfo, isEntityType: !string.IsNullOrEmpty(domainInfo.TypeKey));
            }

            Console.WriteLine("🎉🎉🎉 Done ".Pastel(Color.Green));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.Pastel(Color.Red));
            Console.WriteLine(ex.ToString().Pastel(Color.SlateGray));
        }
    }

    public void GenerateHttpControllerCode(BackendCodeGeneratorCommonCommandOption options)
    {
        Check(options);
        Display(options);

        var typeService = new TypeService(options.SluDir);

        var templateService = new TemplateService(options.Template);

        var controllerProject = FileHelper.GetHttpControllerProjectDirectory(options.SluDir);

        try
        {
            Console.WriteLine($"🚗 Staring generate '{options.Name}' app-service for http api code ...");

            var appServiceInfo = typeService.GetAppContractService(options.Name);
            var httpControllerDefinition = typeService.GetHttpControllerTypeDefinitions();

            var fileContent = templateService.Render("HttpApiController", new
            {
                Namespaces = httpControllerDefinition.ImportNamespaces,
                BaseController = httpControllerDefinition.BaseControllerType,
                projectName = options.ProjectName,
                appService = appServiceInfo,
                routes = GenerateRoute(appServiceInfo.Methods),
            });

            var filePath = Path.Combine(controllerProject.FullName, appServiceInfo.FileProjectPath, $"{appServiceInfo.Name}Controller.cs");

            WriteFileContent(filePath, fileContent, options.Overwrite);

            Console.WriteLine("🎉🎉🎉 Done ".Pastel(Color.Green));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.Pastel(Color.Red));
            Console.WriteLine(ex.ToString().Pastel(Color.SlateGray));
        }
    }

    private static void WriteFileContent(string filePath, string content, bool overwrite = false)
    {
        var directory = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        //#if DEBUG
        //        Console.WriteLine($"⬇️⬇️ File '{filePath}' Content Preview ... ".Pastel(Color.Aqua));
        //        Console.WriteLine(content.Pastel(Color.Aqua));
        //        Console.WriteLine();
        //#endif

        if (File.Exists(filePath) && overwrite == false)
        {
            Console.WriteLine($"➡️ The file '{filePath}' exists.".Pastel(Color.Yellow));
        }
        else
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
            Console.WriteLine($"⬇️ Write file '{filePath}' successful.".Pastel(Color.Green));
        }
    }

    private static string GetSolutionName(string dir)
    {
        var file = Directory.GetFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

        return Path.GetFileNameWithoutExtension(file);
    }

    private static Dictionary<string, AppServiceRouteInfo> GenerateRoute(IEnumerable<TypeMethodInfo> methods)
    {
        var result = new Dictionary<string, AppServiceRouteInfo>();

        foreach (var item in methods)
        {
            string urlpath = null;

            string method = "HttpPost";

            if (item.Name.StartsWith("Create") || item.Name.StartsWith("Add") || item.Name.StartsWith("New"))
            {
                method = "HttpPost";
                urlpath = item.Name.Replace("Create", null).Replace("Add", null).Replace("New", null);
            }
            else if (item.Name.StartsWith("Update"))
            {
                method = "HttpPut";
                urlpath = item.Name.Replace("Update", null);
            }
            else if (item.Name.StartsWith("Delete") || item.Name.StartsWith("Remove"))
            {
                method = "HttpDelete";
                urlpath = item.Name.Replace("Delete", null).Replace("Remove", null);
            }
            else if (item.Name.StartsWith("Get") || item.Name.StartsWith("Load"))
            {
                method = "HttpGet";
                urlpath = item
                    .Name
                    .Replace("GetListWith", null)
                    .Replace("GetList", null)
                    .Replace("List", null)
                    .Replace("GetWith", null)
                    .Replace("Get", null)
                    .Replace("Load", null);
            }
            else
            {
                urlpath = RenderHelperFunctions.ToPluralize(RenderHelperFunctions.ToSlugString(item.Name.Replace("Async", null)));
            }

            if (urlpath != null)
            {
                urlpath = urlpath
                    .Replace("With", null)
                    .Replace("Async", null);
                urlpath = RenderHelperFunctions.ToPluralize(urlpath);
                urlpath = RenderHelperFunctions.ToSlugString(urlpath);
            }

            if (item.Params?.Any() == true)
            {
                List<string> nps = new List<string>();
                string tmp = string.Empty;

                if (item.Params.Any(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)))
                {
                    nps.Add("{id}");
                }

                if (!string.IsNullOrEmpty(urlpath))
                    nps.Add(urlpath);

                foreach (var p in item.Params.Where(x => !x.IsClass && !x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)))
                {
                    nps.Add("{" + RenderHelperFunctions.CamelCase(p.Name) + "}");
                }

                urlpath = string.Join("/", nps);
            }

            if (string.IsNullOrEmpty(urlpath))
                urlpath = null;

            result[item.Name] = new AppServiceRouteInfo(method, urlpath);
        }

        return result;
    }
}
