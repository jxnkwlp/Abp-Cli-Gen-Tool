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

            rootCommand.AddCommand(new CodeGeneratorCommand().GetCommand());

            return await rootCommand.InvokeAsync(args);
        }
    }
}
