using System.CommandLine;
using System.Threading.Tasks;

namespace AbpProjectTools
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand()
            {
                Description = "Abp project tools",
            };

            rootCommand.AddGlobalOption(new Option<string>("--slu-dir", "The solution root dir") { IsRequired = true, });
            rootCommand.AddOption(new Option<bool>("--overwite", () => true));

            rootCommand.AddCommand(new CodeGeneratorCommand(rootCommand.GlobalOptions).GetCommand());

            return await rootCommand.InvokeAsync(args);
        }
    }
}
