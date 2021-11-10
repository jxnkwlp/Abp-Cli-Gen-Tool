using System.CommandLine;
using System.IO;

namespace AbpProjectTools.Commands
{
    public class CodeGeneratorCommand : CommandBase
    {
        public CodeGeneratorCommand()
        {
        }

        public override Command GetCommand()
        {
            var command = new Command("generate", "Code generate");
            command.AddAlias("gen");

            var backendCommand = new Command("backend", "Generate abp repository, CRUD app service, http controller code");

            backendCommand.AddGlobalOption(new Option<string>("--slu-dir", () => Directory.GetCurrentDirectory(), "The solution root dir. Default is current directory") { IsRequired = false, });
            backendCommand.AddGlobalOption(new Option<string>("--name", "The name of entity or app service") { IsRequired = true, });
            backendCommand.AddGlobalOption(new Option<string>("--project-name", () => GetDefaultSolutionName(), "The project name. Default is solution name if found in solution directory") { IsRequired = true, });
            backendCommand.AddGlobalOption(new Option<bool>("--overwrite", () => false, "Over write file if the target file exists"));
            backendCommand.AddGlobalOption(new Option<string>("--templates", "The template files directory"));

            backendCommand.AddCommand(new GenerateDomainServiceCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateRepositoryCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateAppServiceCodeCommand().GetCommand());
            backendCommand.AddCommand(new GenerateHttpControllerCodeCommand().GetCommand());

            command.AddCommand(backendCommand);

            var fontendCommand = new Command("fontend", "Generate ts type and service code");
            fontendCommand.AddCommand(new GenerateTypeScriptCodeCommand().GetCommand());

            command.AddCommand(fontendCommand);

            return command;
        }

        static string GetDefaultSolutionName()
        {
            var slnFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln", SearchOption.TopDirectoryOnly);

            if (slnFiles?.Length == 1)
            {
                var name = Path.GetFileNameWithoutExtension(slnFiles[0]);

                if (name.LastIndexOf('.') > 0)
                {
                    name = name.Substring(name.LastIndexOf('.') + 1);
                }

                return name;
            }

            return null;
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
