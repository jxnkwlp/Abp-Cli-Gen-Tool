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
        var fontendCommand = new Command("fontend", "Generate ts type and service code");
        fontendCommand.AddCommand(new GenerateTypeScriptCodeCommand().GetCommand());

        command.AddCommand(fontendCommand);

        return command;
    }

}
