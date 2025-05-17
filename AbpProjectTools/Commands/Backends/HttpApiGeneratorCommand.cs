using AbpProjectTools.Models;
using AbpProjectTools.Services;
using Pastel;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using System.Drawing;

namespace AbpProjectTools.Commands.Backends;

public class HttpApiGeneratorCommand : ICmdCommand
{
    public Command GetCommand()
    {
        var command = new Command("http-api", "Generate http api service code");

        command.AddBackendCodeGenerateOptions();

        command.AddArgument(new Argument<string[]>("name", "The app service names"));

        command.AddOption(new Option<string>("--base-dir"));
        command.AddOption(new Option<bool>(["--post", "-p"]));


        command.Handler = CommandHandler.Create<BackendCodeHttpApiGenerateOptions>(options =>
        {
            try
            {
                var generator = new BackendCodeGenerator(options);

                generator.GenerateHttpApi(options);

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
