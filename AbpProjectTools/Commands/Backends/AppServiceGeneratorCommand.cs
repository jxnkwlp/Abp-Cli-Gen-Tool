using AbpProjectTools.Models;
using AbpProjectTools.Services;
using Pastel;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.Drawing;

namespace AbpProjectTools.Commands.Backends;

public class AppServiceGeneratorCommand : ICmdCommand
{
    public Command GetCommand()
    {
        var command = new Command("app-service", "Generate app service code");

        command.AddBackendCodeGenerateOptions();

        command.AddArgument(new Argument<string[]>(name: "entity-name", "The entity names"));

        command.AddOption(new Option<bool>(["--empty", "-e"]));
        command.AddOption(new Option<bool>("--crud"));

        command.AddOption(new Option<bool>(["--split-create-update", "-scu"]));
        command.AddOption(new Option<bool>(["--split-result", "-sr"]));

        //command.AddOption(new Option<string>("--list-request-type-name", ""));
        //command.AddOption(new Option<string>("--list-result-type-name", ""));
        //command.AddOption(new Option<string>("--create-type-name", ""));
        //command.AddOption(new Option<string>("--update-type-name", ""));

        command.AddOption(new Option<string>("--base-dir"));


        command.Handler = CommandHandler.Create<BackendCodeAppServiceGenerateOptions>(options =>
        {
            try
            {
                var generator = new BackendCodeGenerator(options);

                generator.GenerateAppService(options);

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
