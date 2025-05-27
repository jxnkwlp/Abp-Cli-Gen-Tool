using AbpProjectTools.Commands.Backends;
using System.CommandLine;

namespace AbpProjectTools.Commands;

public class CodeGeneratorCommand : ICmdCommand
{
    public Command GetCommand()
    {
        var command = new Command("code", "Code generate");

        command.AddCommand(new DomainServiceGeneratorCommand().GetCommand());
        command.AddCommand(new RepositoryGeneratorCommand().GetCommand());
        command.AddCommand(new AppServiceGeneratorCommand().GetCommand());
        command.AddCommand(new HttpApiGeneratorCommand().GetCommand());

        return command;
    }
}
