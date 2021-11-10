using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using Pastel;

namespace AbpProjectTools.Commands
{
    public class GenerateAppServiceCodeCommand : CommandBase
    {
        public override Command GetCommand()
        {
            var command = new Command("app-service", "Generate CRUD app service code");

            command.AddOption(new Option<string>("--list-request-type-name", ""));
            command.AddOption(new Option<string>("--list-result-type-name", ""));
            command.AddOption(new Option<string>("--create-type-name", ""));
            command.AddOption(new Option<string>("--update-type-name", ""));
            command.AddOption(new Option<bool>("--split-list-result-type", () => false, ""));
            command.AddOption(new Option<bool>("--split-cu-type", () => false, ""));
            command.AddOption(new Option<bool>("--basic-service", ""));
            command.AddOption(new Option<bool>("--crud", () => true, ""));

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

                try
                {
                    Console.WriteLine($"🚗 Staring generate '{options.Name}' app service code ...");

                    var domainInfo = typeService.GetDomain(options.Name, true);

                    // service
                    var fileContent = templateService.Render("AppServiceInterface", new
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

                    var filePath = Path.Combine(ContractsProject.FullName, domainInfo.FileProjectPath, $"I{domainInfo.TypeName}AppService.cs");

                    WriteFileContent(filePath, fileContent, options.Overwrite);

                    // service
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
                    if (options.BasicService == false)
                    {
                        void GenerateDto(string typeName, bool isListRequestType = false, bool isCreateType = false, bool isUpdateType = false, bool isEntityType = false, bool isListResultType = false)
                        {
                            fileContent = templateService.Render("AppServiceDto", new
                            {
                                projectName = options.ProjectName,
                                domain = domainInfo,
                                TypeName = typeName,

                                IsCreateType = isCreateType,
                                IsUpdateType = isUpdateType,
                                IsListRequestType = isListRequestType,
                                IsEntityType = isEntityType,
                                isListResultType = isListResultType,
                            });

                            filePath = Path.Combine(ContractsProject.FullName, domainInfo.FileProjectPath, $"{typeName}.cs");

                            WriteFileContent(filePath, fileContent, options.Overwrite);
                        }

                        GenerateDto(options.ListRequestTypeName, isListRequestType: true);

                        if (options.SplitCuType)
                        {
                            GenerateDto(options.CreateTypeName, isCreateType: true);
                            GenerateDto(options.UpdateTypeName, isUpdateType: true);
                        }
                        else
                        {
                            GenerateDto(options.CreateTypeName, isCreateType: true);
                        }

                        if (options.SplitListResultType)
                        {
                            GenerateDto($"{domainInfo.TypeName}BasicDto", isListResultType: true);
                        }

                        GenerateDto($"{domainInfo.TypeName}Dto", isEntityType: true);
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
}
