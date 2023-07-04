using System.CommandLine;
using System.CommandLine.Invocation;
using AbpProjectTools.Services;

namespace AbpProjectTools.Commands;

public class GenerateDomainServiceCodeCommand : CommandBase
{
    public override Command GetCommand()
    {
        var command = new Command("domain-service", "Generate an empty domain service code")
        {
            Handler = CommandHandler.Create<BackendCodeGeneratorCommonCommandOption>(options =>
            {
                new BackendCodeCommandExecutor(options.SluDir).GenerateDomainServiceCode(options);
            })
        };

        return command;
    }
}
