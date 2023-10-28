using System.CommandLine;

namespace AbpProjectTools.Commands;

public class CodeGeneratorCommand : CommandBase
{
    public CodeGeneratorCommand()
    {
    }

    public override Command GetCommand()
    {
        var command = new Command("generate", "Code generate");
        command.AddAlias("gen");

        // backend
        command.AddCommand(new BackendCodeGeneratorCommand().GetCommand());

        // fontend 
        command.AddCommand(new FontendCodeGeneratorCommand().GetCommand());

        return command;
    }

}
