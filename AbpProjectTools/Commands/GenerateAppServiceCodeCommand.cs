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
            var command = new Command("app-service");

            command.AddOption(new Option<string>("--list-request-type-name", ""));
            command.AddOption(new Option<string>("--create-type-name", ""));
            command.AddOption(new Option<string>("--update-type-name", ""));
            command.AddOption(new Option<bool>("--split", () => false, ""));
            command.AddOption(new Option<bool>("--basic-service", ""));

            command.Handler = CommandHandler.Create<GenerateAppServiceCommandOption>(options =>
            {
                if (string.IsNullOrEmpty(options.ListRequestTypeName))
                    options.ListRequestTypeName = $"{options.Name}ListRequestDto";

                if (string.IsNullOrEmpty(options.CreateTypeName))
                    options.CreateTypeName = $"{options.Name}CreateDto";

                if (string.IsNullOrEmpty(options.UpdateTypeName))
                    options.UpdateTypeName = $"{options.Name}UpdateDto";

                if (!options.Split)
                {
                    options.CreateTypeName = $"{options.Name}CreateOrUpdateDto";
                    options.UpdateTypeName = $"{options.Name}CreateOrUpdateDto";
                }


                var typeService = new TypeService(options.SluDir);

                var templateService = new TemplateService(options.Template);

                var ContractsProject = FileHelper.GetApplicationContractProjectDirectory(options.SluDir);
                var appServiceProject = FileHelper.GetApplicationProjectDirectory(options.SluDir);

                try
                {
                    Console.WriteLine($"🚗 Staring generate domain '{options.Name}' app service code ...");

                    var domainInfo = typeService.GetDomain(options.Name, true);

                    // service
                    var fileContent = templateService.Render("AppServiceInterface", new
                    {
                        domain = domainInfo,
                        CreateTypeName = options.CreateTypeName,
                        UpdateTypeName = options.UpdateTypeName,
                        ListRequestTypeName = options.ListRequestTypeName,
                        Split = options.Split,
                        BasicService = options.BasicService,
                    });

                    var filePath = Path.Combine(ContractsProject.FullName, domainInfo.FileProjectPath, $"I{domainInfo.TypeName}AppService.cs");

                    WriteFileContent(filePath, fileContent, options.Overwrite);

                    // service
                    fileContent = templateService.Render("AppServiceService", new
                    {
                        domain = domainInfo,
                        CreateTypeName = options.CreateTypeName,
                        UpdateTypeName = options.UpdateTypeName,
                        ListRequestTypeName = options.ListRequestTypeName,
                        Split = options.Split,
                        BasicService = options.BasicService,
                    });

                    filePath = Path.Combine(appServiceProject.FullName, domainInfo.FileProjectPath, $"{domainInfo.TypeName}AppService.cs");

                    WriteFileContent(filePath, fileContent, options.Overwrite);




                    // dto
                    if (options.BasicService == false)
                    {
                        void GenerateDto(string typeName, bool isListRequestType = false, bool isCreateType = false, bool isUpdateType = false, bool isEntityType = false)
                        {
                            fileContent = templateService.Render("AppServiceDto", new
                            {
                                domain = domainInfo,
                                TypeName = typeName,

                                IsCreateType = isCreateType,
                                IsUpdateType = isUpdateType,
                                IsListRequestType = isListRequestType,
                                IsEntityType = isEntityType,
                            });

                            filePath = Path.Combine(ContractsProject.FullName, domainInfo.FileProjectPath, $"{typeName}.cs");

                            WriteFileContent(filePath, fileContent, options.Overwrite);
                        }

                        GenerateDto(options.ListRequestTypeName, isListRequestType: true);
                        GenerateDto($"{domainInfo.TypeName}Dto", isEntityType: true);

                        if (options.Split)
                        {
                            GenerateDto(options.CreateTypeName, isCreateType: true);
                            GenerateDto(options.UpdateTypeName, isUpdateType: true);
                        }
                        else
                        {
                            GenerateDto(options.CreateTypeName, isCreateType: true);
                        }
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
