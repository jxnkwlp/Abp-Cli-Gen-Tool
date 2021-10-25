using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using Pastel;

namespace AbpProjectTools.Commands
{
    public class GenerateHttpControllerCodeCommand : CommandBase
    {
        public override Command GetCommand()
        {
            var command = new Command("http-controller");

            command.Handler = CommandHandler.Create<BackendCodeGeneratorCommonCommandOption>(options =>
            {
                var typeService = new TypeService(options.SluDir);

                var templateService = new TemplateService(options.Template);

                var controllerProject = FileHelper.GetHttpControllerProjectDirectory(options.SluDir);

                try
                {
                    Console.WriteLine($"🚗 Staring generate domain '{options.Name}' app-service for http api code ...");

                    var domainInfo = typeService.GetDomain(options.Name);
                    var appServiceInfo = typeService.GetAppServiceContract(options.Name);

                    var fileContent = templateService.Render("HttpApiController", new
                    {
                        projectName = options.ProjectName,
                        domain = domainInfo,
                        appServices = appServiceInfo.Methods,
                        routes = GenerateRoute(appServiceInfo.Methods),
                    });

                    var filePath = Path.Combine(controllerProject.FullName, domainInfo.FileProjectPath, $"{domainInfo.TypeName}Controller.cs");

                    WriteFileContent(filePath, fileContent, options.Overwrite);

                    Console.WriteLine("🎉 Done. ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.Pastel(Color.Red));
                    Console.WriteLine(ex.ToString().Pastel(Color.Red));
                }

            });

            return command;
        }

        private static Dictionary<string, AppServiceRouteInfo> GenerateRoute(IEnumerable<TypeMethodInfo> methods)
        {
            var result = new Dictionary<string, AppServiceRouteInfo>();

            foreach (var item in methods)
            {
                if (item.Name == "CreateAsync")
                {
                    result[item.Name] = new AppServiceRouteInfo("HttpPost");
                }
                else if (item.Name == "UpdateAsync")
                {
                    result[item.Name] = new AppServiceRouteInfo("HttpPut", "{id}");
                }
                else if (item.Name == "DeleteAsync")
                {
                    result[item.Name] = new AppServiceRouteInfo("HttpDelete", "{id}");
                }
                else if (item.Name == "GetAsync")
                {
                    result[item.Name] = new AppServiceRouteInfo("HttpGet", "{id}");
                }
                else if (item.Name == "GetListAsync")
                {
                    result[item.Name] = new AppServiceRouteInfo("HttpGet");
                }
                else
                {
                    result[item.Name] = new AppServiceRouteInfo("HttpPost", item.Name.Replace("Async", "").ToLower());
                }
            }

            return result;
        }
    }
}
