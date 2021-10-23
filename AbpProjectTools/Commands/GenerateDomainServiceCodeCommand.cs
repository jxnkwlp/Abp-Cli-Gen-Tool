using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using Pastel;

namespace AbpProjectTools.Commands
{
    public class GenerateDomainServiceCodeCommand : CommandBase
    {
        public override Command GetCommand()
        {
            var command = new Command("domain-service");

            command.Handler = CommandHandler.Create<BackendCodeGeneratorCommonCommandOption>(options =>
            {
                var typeService = new TypeService(options.SluDir);

                var templateService = new TemplateService(options.Template);

                try
                {
                    Console.WriteLine($"🚗 Staring generate domain '{options.Name}' service code ...");

                    var domainInfo = typeService.GetDomain(options.Name);

                    var fileContent = templateService.Render("DomainService", domainInfo);

                    var filePath = Path.Combine(domainInfo.FileDirectory, $"{domainInfo.TypeName}Manager.cs");

                    WriteFileContent(filePath, fileContent, options.Overwrite);

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
