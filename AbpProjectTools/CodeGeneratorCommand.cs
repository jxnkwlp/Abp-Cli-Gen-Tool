using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

namespace AbpProjectTools
{
    public class CodeGeneratorCommand
    {
        private readonly IReadOnlyList<Option> _options;

        public CodeGeneratorCommand(IReadOnlyList<Option> options)
        {
            _options = options;
        }

        public Command GetCommand()
        {
            var command = new Command("generate");
            command.AddAlias("gen");

            command.AddCommand(GenerateRepository());
            command.AddCommand(GenerateDomainService());
            command.AddCommand(GenerateAppService());
            command.AddCommand(GenerateHttpController());

            return command;
        }

        private Command GenerateRepository()
        {
            var command = new Command("repository");

            command.AddOption(new Option<string>("--name", "The Domain entity name") { IsRequired = true, });

            command.Handler = CommandHandler.Create<GenerateRepositoryCommandOption>(options =>
            {
                try
                {
                    Console.WriteLine($"😁 Staring generate domain '{options.Name}' repository...");
                    Console.WriteLine();

                    var domainFile = FileHelper.FindFile(options.SluDir, options.Name);

                    var domainProjectPath = FileHelper.GetDomainProjectDirectory(options.SluDir);
                    var efCoreProjectPath = FileHelper.GetEntityFrameworkCoreProjectDirectory(options.SluDir);

                    Console.WriteLine($"Domain project path: {domainProjectPath.FullName}");
                    Console.WriteLine($"EfCore project path: {efCoreProjectPath.FullName}");
                    Console.WriteLine();

                    var domain = TypeHelper.GetDomain(domainProjectPath, options.Name);
                    domain.FileDirectory = domainFile.DirectoryName.Substring(domainProjectPath.FullName.Length + 1);
                    domain.FileFullName = domainFile.FullName;
                    domain.ProjectName = GetSolutionName(options.SluDir);

                    Console.WriteLine($"Domain find in '{domain.FileDirectory}', type: '{domain.TypeFullName}' ");
                    Console.WriteLine();

                    var efContext = TypeHelper.GetEfCore(efCoreProjectPath);

                    Console.WriteLine($"EfCore Dbcontext find : {efContext.TypeFullName} ");
                    Console.WriteLine();

                    // write interface 
                    var code1 = CodeGenerator.GenerateRepositoryInterface(domain);
                    var file1 = Path.Combine(domainFile.DirectoryName, $"I{domain.TypeName}Repository.cs");

                    Console.WriteLine($"Generate file to: {file1} ...");
                    Console.WriteLine();

                    if (File.Exists(file1) && options.Overwite == false)
                        Console.WriteLine($"The file '{file1}' exists.");
                    else
                        File.WriteAllText(file1, code1);

                    var code2 = CodeGenerator.GenerateEfCoreRepository(efContext, domain);
                    var file2 = Path.Combine(efCoreProjectPath.FullName, "EntityFrameworkCore", "Repositories", $"{domain.TypeName}Repository.cs");

                    Console.WriteLine($"Generate file to: {file2} ...");
                    Console.WriteLine();

                    if (!Directory.Exists(Path.GetDirectoryName(file2)))
                        Directory.CreateDirectory(Path.GetDirectoryName(file2));

                    if (File.Exists(file2) && options.Overwite == false)
                        Console.WriteLine($"The file '{file2}' exists.");
                    else
                        File.WriteAllText(file2, code2);


                    Console.WriteLine("🎉 Done. ");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            return command;
        }

        private Command GenerateDomainService()
        {
            var command = new Command("domain-service");

            command.AddOption(new Option<string>("--name", "The Domain entity name") { IsRequired = true, });


            command.Handler = CommandHandler.Create<GenerateRepositoryCommandOption>(options =>
            {
                try
                {
                    Console.WriteLine($"😁 Staring generate domain '{options.Name}' service...");
                    Console.WriteLine();

                    var domainFile = FileHelper.FindFile(options.SluDir, options.Name);

                    var domainProjectPath = FileHelper.GetDomainProjectDirectory(options.SluDir);

                    Console.WriteLine($"Domain project path: {domainProjectPath.FullName}");
                    Console.WriteLine();


                    var domain = TypeHelper.GetDomain(domainProjectPath, options.Name);
                    domain.FileDirectory = domainFile.DirectoryName.Substring(domainProjectPath.FullName.Length + 1);
                    domain.FileFullName = domainFile.FullName;
                    domain.ProjectName = GetSolutionName(options.SluDir);

                    Console.WriteLine($"Domain find in '{domain.FileDirectory}', type: '{domain.TypeFullName}' ");
                    Console.WriteLine();


                    // write file 
                    var code1 = CodeGenerator.GenerateDomainService(domain);
                    var file1 = Path.Combine(domainFile.DirectoryName, $"{domain.TypeName}Manager.cs");

                    FileWrite(file1, code1, options.Overwite);
                    Console.WriteLine($"Write file '{file1}'. ");

                    if (File.Exists(file1) && options.Overwite == false)
                        Console.WriteLine($"The file '{file1}' exists.");
                    else
                        File.WriteAllText(file1, code1);

                    Console.WriteLine("🎉 Done. ");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            return command;
        }

        private Command GenerateAppService()
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

        private Command GenerateHttpController()
        {
            var command = new Command("http-controller");

            command.AddOption(new Option<string>("--name", "The Domain entity name") { IsRequired = true, });


            command.Handler = CommandHandler.Create<GenerateRepositoryCommandOption>(options =>
            {
                try
                {
                    Console.WriteLine($"😁 Staring generate domain '{options.Name}' service...");
                    Console.WriteLine();

                    var domainFile = FileHelper.FindFile(options.SluDir, options.Name);

                    var webProjectPath = FileHelper.GetWebProjectDirectory(options.SluDir);
                    var domainProjectPath = FileHelper.GetDomainProjectDirectory(options.SluDir);
                    var appContractProjectPath = FileHelper.GetApplicationContractProjectDirectory(options.SluDir);
                    var httpApiProjectPath = FileHelper.GetHttpControllerProjectDirectory(options.SluDir);

                    Console.WriteLine($"Domain project path: {domainProjectPath.FullName}");
                    Console.WriteLine($"Httpapi project path: {httpApiProjectPath.FullName}");
                    Console.WriteLine();

                    var domain = TypeHelper.GetDomain(domainProjectPath, options.Name);
                    domain.FileDirectory = domainFile.DirectoryName.Substring(domainProjectPath.FullName.Length + 1);
                    domain.FileFullName = domainFile.FullName;
                    domain.ProjectName = GetSolutionName(options.SluDir);

                    Console.WriteLine($"Domain find in '{domain.FileDirectory}', type: '{domain.TypeFullName}' ");
                    Console.WriteLine();


                    // write file 
                    // var body = TypeHelper.GenerateHttpApiController(appContractProjectPath, webProjectPath.FullName, domain.TypeName);
                    var code = CodeGenerator.GenerateHttpApiController(domain, null);
                    var file = Path.Combine(httpApiProjectPath.FullName, domain.FileDirectory, $"{domain.TypeName}Controller.cs");

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


        private static void FileWrite(string file, string content, bool overwite = false)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));

            if (File.Exists(file) && overwite == false)
                Console.WriteLine($"The file '{file}' exists.");
            else
                File.WriteAllText(file, content);
        }

        private static string GetSolutionName(string dir)
        {
            var file = Directory.GetFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

            return Path.GetFileNameWithoutExtension(file);
        }
    }

    public class CodeGeneratorCommandOption
    {
        public string Name { get; set; }
        public string SluDir { get; set; }
        public bool Overwite { get; set; }
    }

    public class GenerateRepositoryCommandOption : CodeGeneratorCommandOption
    {
    }

    public class GenerateAppServiceCommandOption : CodeGeneratorCommandOption
    {
        public string ListRequestTypeName { get; set; }
        public string CreateTypeName { get; set; }
        public string UpdateTypeName { get; set; }
        public bool CreateSameAsUpdate { get; set; }

        /// <summary>
        ///  custom app service 
        /// </summary>
        public bool Custom { get; set; }
    }
}
