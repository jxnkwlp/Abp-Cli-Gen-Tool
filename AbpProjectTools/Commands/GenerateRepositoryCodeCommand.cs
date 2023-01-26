using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using AbpProjectTools.Services;
using Pastel;

namespace AbpProjectTools.Commands;

public class GenerateRepositoryCodeCommand : CommandBase
{
    public override Command GetCommand()
    {
        var command = new Command("repository", "Generate an domain repository (include efcore repository)")
        {
            Handler = CommandHandler.Create<BackendCodeGeneratorCommonCommandOption>(options =>
            {
                var typeService = new TypeService(options.SluDir);

                var templateService = new TemplateService(options.Template);

                try
                {
                    Console.WriteLine($"🚗 Staring generate domain '{options.Name}' repository code ..");

                    var domainInfo = typeService.GetDomain(options.Name);
                    var efInfo = typeService.GetEfCore();

                    // file 1
                    var fileContent = templateService.Render("DomainRepository", domainInfo);
                    var filePath = Path.Combine(domainInfo.FileDirectory, $"I{domainInfo.TypeName}Repository.cs");

                    WriteFileContent(filePath, fileContent, options.Overwrite);


                    // file 2 
                    fileContent = templateService.Render("EfCoreRepository", new { domain = domainInfo, ef = efInfo });
                    filePath = Path.Combine(efInfo.FileDirectoryName, "Repositories", $"{domainInfo.TypeName}Repository.cs");

                    WriteFileContent(filePath, fileContent, options.Overwrite);

                    Console.WriteLine("🎉 Done. ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.Pastel(Color.Red));
                    throw;
                }
            })
        };

        return command;
    }
}
