using System;
using System.CommandLine;
using System.Text;
using System.Threading.Tasks;

namespace AbpProjectTools
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var rootCommand = new RootCommand()
            {
                Description = "Abp project tools",
            };

            rootCommand.AddGlobalOption(new Option<string>("--slu-dir", "The solution root dir") { IsRequired = true, });
            rootCommand.AddOption(new Option<bool>("--overwite", () => false));
            rootCommand.AddOption(new Option<string>("--templates", "The template files directory"));

            rootCommand.AddCommand(new CodeGeneratorCommand().GetCommand());

            return await rootCommand.InvokeAsync(args);
        }
    }
}
