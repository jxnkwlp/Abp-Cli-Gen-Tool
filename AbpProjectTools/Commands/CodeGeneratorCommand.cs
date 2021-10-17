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

            command.AddCommand(new GenerateAppServiceCodeCommand().GetCommand());
            command.AddCommand(new GenerateDomainServiceCodeCommand().GetCommand());
            command.AddCommand(new GenerateHttpControllerCodeCommand().GetCommand());
            command.AddCommand(new GenerateRepositoryCodeCommand().GetCommand());


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
