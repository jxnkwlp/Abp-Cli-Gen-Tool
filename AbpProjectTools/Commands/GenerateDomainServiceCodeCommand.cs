using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace AbpProjectTools.Commands
{
    public class GenerateDomainServiceCodeCommand : CommandBase
    {
        public override Command GetCommand()
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
    }
}
