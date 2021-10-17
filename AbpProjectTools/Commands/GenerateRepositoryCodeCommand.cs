using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace AbpProjectTools.Commands
{
    public class GenerateRepositoryCodeCommand : CommandBase
    {
        public override Command GetCommand()
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
    }
}
