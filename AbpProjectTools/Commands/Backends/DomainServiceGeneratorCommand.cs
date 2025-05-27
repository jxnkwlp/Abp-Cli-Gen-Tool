using AbpProjectTools.Models;
using AbpProjectTools.Services;
using Pastel;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.Drawing;

namespace AbpProjectTools.Commands.Backends;

public class DomainServiceGeneratorCommand : ICmdCommand
{
    public Command GetCommand()
    {
        var command = new Command("domain", "Generate domain entity service & repository code");

        command.AddBackendCodeGenerateOptions();

        command.AddArgument(new Argument<string[]>(name: "entity-name", "The entity names"));

        command.AddOption(new Option<bool>("--repository", () => true, "Generate repository"));
        command.AddOption(new Option<bool>("--readonly-repo", "Generate readonly repository"));
        command.AddOption(new Option<bool>("--interface", () => true, "Use domain manager interface"));
        command.AddOption(new Option<bool>("--ef", () => true, "Use EF Core"));
        command.AddOption(new Option<bool>("--mongo", () => true, "Use MongoDB"));

        command.Handler = CommandHandler.Create<BackendCodeDomainGenerateOptions>(options =>
        {
            try
            {
                var generator = new BackendCodeGenerator(options);
                generator.GenerateDomain(options);

                if (options.Repository)
                    generator.GenerateRepository(options);

                Console.WriteLine("\r\n🎉🎉🎉 Done ".Pastel(Color.Green));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

                ex.Print();
            }
        });

        return command;
    }
}
