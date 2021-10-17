using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace AbpProjectTools.Commands
{
    public class GenerateHttpControllerCodeCommand : CommandBase
    {
        public override Command GetCommand()
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
    }
}
