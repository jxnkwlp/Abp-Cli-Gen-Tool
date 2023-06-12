using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using System.Linq;
using AbpProjectTools.Services;
using Pastel;

namespace AbpProjectTools.Commands;

public class GenerateAppServiceCodeCommand : CommandBase
{
    public override Command GetCommand()
    {
        var command = new Command("app-service", "Generate CRUD app service code");

        command.AddOption(new Option<string>("--list-request-type-name", ""));
        command.AddOption(new Option<string>("--list-result-type-name", ""));
        command.AddOption(new Option<string>("--create-type-name", ""));
        command.AddOption(new Option<string>("--update-type-name", ""));
        command.AddOption(new Option<string>("--project-name", ""));
        command.AddOption(new Option<bool>("--split-list-result-type", () => false, ""));
        command.AddOption(new Option<bool>("--split-cu-type", () => false, ""));
        command.AddOption(new Option<bool>("--basic-service", ""));
        command.AddOption(new Option<bool>("--crud", () => false, ""));

        command.Handler = CommandHandler.Create<GenerateAppServiceCommandOption>(options =>
        {
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

            if (string.IsNullOrWhiteSpace(options.ProjectName))
                options.ProjectName = typeService.GetSlutionName();

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

                Console.WriteLine("🎉 Done. ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.Pastel(Color.Red));
            }
        });

        return command;
    }
}
