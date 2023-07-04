using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using AbpProjectTools.Services;

namespace AbpProjectTools.Commands;

public class BackendCodeGeneratorCommand : CommandBase
{
    public override Command GetCommand()
    {
        // backend
        var backendCommand = new Command("backend", "Generate abp repository, CRUD app service, http controller code");

        backendCommand.AddGlobalOption(new Option<string>("--slu-dir", () => Directory.GetCurrentDirectory(), "The solution root dir. Default is current directory") { IsRequired = false, });
        backendCommand.AddGlobalOption(new Option<string>("--project-name", () => GetDefaultSolutionName(), "The project name. Default is solution name if found in solution directory") { IsRequired = true, });
        backendCommand.AddGlobalOption(new Option<string>("--name", "The name of entity or app service") { IsRequired = true, });
        backendCommand.AddGlobalOption(new Option<bool>("--overwrite", () => false, "Over write file if the target file exists"));
        backendCommand.AddGlobalOption(new Option<string>("--templates", "The template files directory"));

        // backendCommand.AddCommand(GetGenerateAllCommand());
        backendCommand.AddCommand(GetDomainServiceCommand());
        backendCommand.AddCommand(GetReposityCommand());
        backendCommand.AddCommand(GetAppServiceCommand());
        backendCommand.AddCommand(GetHttpControllerCommand());

        return backendCommand;
    }

    private Command GetGenerateAllCommand()
    {
        return new Command("all", "Generate all code for this entity")
        {
            Handler = CommandHandler.Create<BackendAppServiceCodeGeneratorCommandOption>(options =>
            {
                new BackendCodeCommandExecutor(options.SluDir).GenerateAllCode(options);
            })
        };
    }

    private Command GetDomainServiceCommand()
    {
        return new Command("domain-service", "Generate an empty domain service code")
        {
            Handler = CommandHandler.Create<BackendCodeGeneratorCommonCommandOption>(options =>
            {
                new BackendCodeCommandExecutor(options.SluDir).GenerateDomainServiceCode(options);
            })
        };
    }

    private Command GetReposityCommand()
    {
        return new Command("repository", "Generate an domain repository (include efcore repository)")
        {
            Handler = CommandHandler.Create<BackendCodeGeneratorCommonCommandOption>(options =>
            {
                new BackendCodeCommandExecutor(options.SluDir).GenerateRepositoryCode(options);
            })
        };
    }

    private Command GetAppServiceCommand()
    {
        var command = new Command("app-service", "Generate CRUD app service code");

        command.AddOption(new Option<string>("--list-request-type-name", ""));
        command.AddOption(new Option<string>("--list-result-type-name", ""));
        command.AddOption(new Option<string>("--create-type-name", ""));
        command.AddOption(new Option<string>("--update-type-name", ""));
        command.AddOption(new Option<string>("--project-name", ""));
        command.AddOption(new Option<bool>("--split-list-result-type", () => false, ""));
        command.AddOption(new Option<bool>("--split-cu-type", () => false, ""));
        command.AddOption(new Option<bool>("--basic-service", ""));
        command.AddOption(new Option<bool>("--crud", () => false, ""));

        command.Handler = CommandHandler.Create<BackendAppServiceCodeGeneratorCommandOption>(options =>
        {
            new BackendCodeCommandExecutor(options.SluDir).GenerateAppServiceCode(options);
        });

        return command;
    }

    private Command GetHttpControllerCommand()
    {
        return new Command("http-controller", "Generate http controller code from app service")
        {
            Handler = CommandHandler.Create<BackendCodeGeneratorCommonCommandOption>(options =>
            {
                new BackendCodeCommandExecutor(options.SluDir).GenerateHttpControllerCode(options);
            })
        };
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

public class BackendAppServiceCodeGeneratorCommandOption : BackendCodeGeneratorCommonCommandOption
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
