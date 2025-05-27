using System.CommandLine;
using System.IO;

namespace AbpProjectTools;

public static class GlobalOptions
{
    public static void AddBackendCodeGenerateOptions(this Command command)
    {
        // BackendCodeGenerateGlobalOptions
        command.AddOption(new Option<string>("--slu-dir", () => Directory.GetCurrentDirectory(), "The directory of the solution project"));
        command.AddOption(new Option<string>("--slu-name", "The name of the solution project"));
        command.AddOption(new Option<bool>(new[] { "--force", "-f" }, () => false, "Force to overwrite the existing files"));

        command.AddOption(new Option<string>("--domain-project-name", "The name of the ABP domain project"));
        command.AddOption(new Option<string>("--ef-project-name", "The name of the ABP EF Core project"));
        command.AddOption(new Option<string>("--mongodb-project-name", "The name of the ABP MongoDB project"));
        command.AddOption(new Option<string>("--app-contract-project-name", "The name of the ABP app contract project"));
        command.AddOption(new Option<string>("--app-service-project-name", "The name of the ABP app service project"));
        command.AddOption(new Option<string>("--http-api-project-name", "The name of the ABP Http API project"));

    }
}
