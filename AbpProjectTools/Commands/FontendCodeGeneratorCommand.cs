using System.CommandLine;

namespace AbpProjectTools.Commands;

public class FontendCodeGeneratorCommand : CommandBase
{
    public override Command GetCommand()
    {
        var command = new Command("fontend", "Generate fontend type and service code");

        command.AddCommand(new FontendServiceCodeGeneratorCommand().GetCommand());
        command.AddCommand(new FontendCrudCodeGeneratorCommand().GetCommand());

        return command;
    }
}
