using System;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AbpProjectTools.Commands;

namespace AbpProjectTools;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine($"🚩 Current Directory: {Directory.GetCurrentDirectory()}");
        // Console.WriteLine($"App Directory: {AppContext.BaseDirectory}");

        var rootCommand = new RootCommand()
        {
            Description = "Abp project tools",
            Name = "abptool",
        };

        rootCommand.AddCommand(new CodeGeneratorCommand().GetCommand());

        return await rootCommand.InvokeAsync(args);
    }
}
