using System.CommandLine;

namespace AbpProjectTools.Commands
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
            backendCommand.AddGlobalOption(new Option<string>("--name", "The Domain entity name") { IsRequired = true, });
            backendCommand.AddGlobalOption(new Option<string>("--project-name", "The project name") { IsRequired = true, });
            backendCommand.AddGlobalOption(new Option<bool>("--overwrite", () => false));
            backendCommand.AddGlobalOption(new Option<string>("--templates", "The template files directory"));

            backendCommand.AddCommand(new GenerateDomainServiceCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateRepositoryCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateAppServiceCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateHttpControllerCodeCommand().GetCommand());

            command.AddCommand(backendCommand);

            var fontendCommand = new Command("fontend");
            fontendCommand.AddCommand(new GenerateTypeScriptCodeCommand().GetCommand());

            command.AddCommand(fontendCommand);

            return command;
        }

    }

    public class BackendCodeGeneratorCommonCommandOption
    {
        public string SluDir { get; set; }
        public string Name { get; set; }
        public string ProjectName { get; set; }
        public bool Overwrite { get; set; }
        public string Template { get; set; }
    }

    public class GenerateAppServiceCommandOption : BackendCodeGeneratorCommonCommandOption
    {
        public string ListRequestTypeName { get; set; }
        public string ListResultTypeName { get; set; }
        public string CreateTypeName { get; set; }
        public string UpdateTypeName { get; set; }

        public bool SplitCuType { get; set; }
        public bool SplitListResultType { get; set; }

        public bool Crud { get; set; }

        /// <summary>
        ///  custom app service 
        /// </summary>
        public bool BasicService { get; set; }
    }
}
