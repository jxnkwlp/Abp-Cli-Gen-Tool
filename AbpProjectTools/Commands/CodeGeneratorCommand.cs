using System.CommandLine;
using AbpProjectTools.Commands;

namespace AbpProjectTools
{
    public class CodeGeneratorCommand : CommandBase
    {
        public CodeGeneratorCommand()
        {
        }

        public override Command GetCommand()
        {
            var command = new Command("generate");
            command.AddAlias("gen");

            var backendCommand = new Command("backend");

            backendCommand.AddGlobalOption(new Option<string>("--slu-dir", "The solution root dir") { IsRequired = true, });
            backendCommand.AddOption(new Option<bool>("--overwite", () => false));
            backendCommand.AddOption(new Option<string>("--templates", "The template files directory"));

            backendCommand.AddCommand(new GenerateAppServiceCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateDomainServiceCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateHttpControllerCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateRepositoryCodeCommand().GetCommand());

            command.AddCommand(backendCommand);

            var fontendCommand = new Command("fontend");
            fontendCommand.AddCommand(new GenerateTypeScriptCodeCommand().GetCommand());

            command.AddCommand(fontendCommand);

            return command;
        }

    }

    public class CodeGeneratorCommandOption
    {
        public string Name { get; set; }
        public string SluDir { get; set; }
        public bool Overwite { get; set; }
    }

    public class GenerateRepositoryCommandOption : CodeGeneratorCommandOption
    {
    }

    public class GenerateAppServiceCommandOption : CodeGeneratorCommandOption
    {
        public string ListRequestTypeName { get; set; }
        public string CreateTypeName { get; set; }
        public string UpdateTypeName { get; set; }
        public bool CreateSameAsUpdate { get; set; }

        /// <summary>
        ///  custom app service 
        /// </summary>
        public bool Custom { get; set; }
    }
}
