using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace AbpProjectTools.Commands
{
    public class GenerateAppServiceCodeCommand : CommandBase
    {
        public override Command GetCommand()
        {
            var command = new Command("app-service");

            command.AddOption(new Option<string>("--name", "The Domain entity name") { IsRequired = true, });

            command.AddOption(new Option<string>("--list-request-type-name", ""));
            command.AddOption(new Option<string>("--create-type-name", ""));
            command.AddOption(new Option<string>("--update-type-name", ""));
            command.AddOption(new Option<string>("--create-same-as-update", ""));
            command.AddOption(new Option<bool>("--custom", ""));

            command.Handler = CommandHandler.Create<GenerateAppServiceCommandOption>(options =>
            {
                if (string.IsNullOrEmpty(options.ListRequestTypeName))
                    options.ListRequestTypeName = $"{options.Name}ListRequestDto";

                if (string.IsNullOrEmpty(options.CreateTypeName))
                    options.CreateTypeName = $"{options.Name}CreateDto";

                if (string.IsNullOrEmpty(options.UpdateTypeName))
                    options.UpdateTypeName = $"{options.Name}UpdateDto";

                if (options.CreateSameAsUpdate)
                {
                    options.CreateTypeName = $"{options.Name}CreateOrUpdateDto";
                    options.UpdateTypeName = $"{options.Name}CreateOrUpdateDto";
                }

                try
                {
                    Console.WriteLine($"😁 Staring generate app service '{options.Name}' service...");
                    Console.WriteLine();

                    var domainFile = FileHelper.FindFile(options.SluDir, options.Name);

                    var domainProjectPath = FileHelper.GetDomainProjectDirectory(options.SluDir);
                    var appContractProjectPath = FileHelper.GetApplicationContractProjectDirectory(options.SluDir);
                    var appServiceProjectPath = FileHelper.GetApplicationProjectDirectory(options.SluDir);

                    Console.WriteLine($"Domain project path: {domainProjectPath.FullName}");
                    Console.WriteLine($"App contract service project path: {appContractProjectPath.FullName}");
                    Console.WriteLine($"App service project path: {appServiceProjectPath.FullName}");
                    Console.WriteLine();

                    var domain = TypeHelper.GetDomain(domainProjectPath, options.Name);
                    domain.FileDirectory = domainFile.DirectoryName.Substring(domainProjectPath.FullName.Length + 1);
                    domain.FileFullName = domainFile.FullName;
                    domain.ProjectName = GetSolutionName(options.SluDir);

                    Console.WriteLine($"Domain find in '{domain.FileDirectory}', type: '{domain.TypeFullName}' ");
                    Console.WriteLine();


                    // app service interface 

                    // interface 
                    var code = options.Custom ?
                        CodeGenerator.GenerateAppServiceBasicInterface(domain) :
                        CodeGenerator.GenerateAppServiceCrudInterface(domain, options.ListRequestTypeName, options.CreateTypeName, options.UpdateTypeName);
                    var file = Path.Combine(appContractProjectPath.FullName, domain.FileDirectory, $"I{domain.TypeName}AppService.cs");
                    FileWrite(file, code, options.Overwite);
                    Console.WriteLine($"Write file '{file}'. ");

                    if (!options.Custom)
                    {
                        // entity dto
                        var entityBody = TypeHelper.GetProptiesString(domainProjectPath, options.Name);
                        code = CodeGenerator.GenerateAppServiceEntityDto(domain, entityBody);
                        file = Path.Combine(appContractProjectPath.FullName, domain.FileDirectory, $"{domain.TypeName}Dto.cs");
                        FileWrite(file, code, options.Overwite);
                        Console.WriteLine($"Write file '{file}'. ");

                        // list request dto
                        code = CodeGenerator.GenerateAppServiceListRequestDto(domain);
                        file = Path.Combine(appContractProjectPath.FullName, domain.FileDirectory, $"{domain.TypeName}ListRequestDto.cs");
                        FileWrite(file, code, options.Overwite);
                        Console.WriteLine($"Write file '{file}'. ");

                        // create or update dto
                        if (options.CreateSameAsUpdate)
                        {
                            code = CodeGenerator.GenerateAppServiceEntityDto(domain, entityBody, false, $"{domain.TypeName}CreateOrUpdate");
                            file = Path.Combine(appContractProjectPath.FullName, domain.FileDirectory, $"{domain.TypeName}CreateOrUpdateDto.cs");
                            FileWrite(file, code, options.Overwite);
                            Console.WriteLine($"Write file '{file}'. ");
                        }
                        else
                        {
                            code = CodeGenerator.GenerateAppServiceEntityDto(domain, entityBody, false, $"{domain.TypeName}Create");
                            file = Path.Combine(appContractProjectPath.FullName, domain.FileDirectory, $"{domain.TypeName}CreateDto.cs");
                            FileWrite(file, code, options.Overwite);
                            Console.WriteLine($"Write file '{file}'. ");

                            code = CodeGenerator.GenerateAppServiceEntityDto(domain, entityBody, false, $"{domain.TypeName}Update");
                            file = Path.Combine(appContractProjectPath.FullName, domain.FileDirectory, $"{domain.TypeName}UpdateDto.cs");
                            FileWrite(file, code, options.Overwite);
                            Console.WriteLine($"Write file '{file}'. ");
                        }

                    }

                    // app service
                    code = options.Custom ?
                                CodeGenerator.GenerateAppServiceBasicService(domain) :
                                CodeGenerator.GenerateAppServiceCrudService(domain, options.ListRequestTypeName, options.CreateTypeName, options.UpdateTypeName);
                    file = Path.Combine(appServiceProjectPath.FullName, domain.FileDirectory, $"{domain.TypeName}AppService.cs");
                    FileWrite(file, code, options.Overwite);
                    Console.WriteLine($"Write file '{file}'. ");

                    Console.WriteLine("🎉 Done. ");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            return command;
        }
    }
}
