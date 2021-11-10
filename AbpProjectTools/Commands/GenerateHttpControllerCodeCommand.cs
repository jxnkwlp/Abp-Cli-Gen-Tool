using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using System.Linq;
using Pastel;

namespace AbpProjectTools.Commands
{
    public class GenerateHttpControllerCodeCommand : CommandBase
    {
        public override Command GetCommand()
        {
            var command = new Command("http-controller","Generate http controller code from app service")
            {
                Handler = CommandHandler.Create<BackendCodeGeneratorCommonCommandOption>(options =>
                {
                    var typeService = new TypeService(options.SluDir);

                    var templateService = new TemplateService(options.Template);

                    var controllerProject = FileHelper.GetHttpControllerProjectDirectory(options.SluDir);

                    try
                    {
                        Console.WriteLine($"🚗 Staring generate '{options.Name}' app-service for http api code ...");

                        var appServiceInfo = typeService.GetAppServiceContract(options.Name);

                        var fileContent = templateService.Render("HttpApiController", new
                        {
                            projectName = options.ProjectName,
                            appService = appServiceInfo,
                            routes = GenerateRoute(appServiceInfo.Methods),
                        });

                        var filePath = Path.Combine(controllerProject.FullName, appServiceInfo.FileProjectPath, $"{appServiceInfo.Name}Controller.cs");

                        WriteFileContent(filePath, fileContent, options.Overwrite);

                        Console.WriteLine("🎉 Done. ");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.Pastel(Color.Red));
                        Console.WriteLine(ex.ToString().Pastel(Color.Red));
                    }

                })
            };

            return command;
        }

        private static Dictionary<string, AppServiceRouteInfo> GenerateRoute(IEnumerable<TypeMethodInfo> methods)
        {
            var result = new Dictionary<string, AppServiceRouteInfo>();

            foreach (var item in methods)
            {
                string urlpath = null;

                string method = "HttpPost";

                if (item.Name.StartsWith("Create") || item.Name.StartsWith("Add") || item.Name.StartsWith("New"))
                {
                    method = "HttpPost";
                    urlpath = item.Name.Replace("Create", null).Replace("Add", null).Replace("New", null);
                }
                else if (item.Name.StartsWith("Update"))
                {
                    method = "HttpPut";
                    urlpath = item.Name.Replace("Update", null);
                }
                else if (item.Name.StartsWith("Delete") || item.Name.StartsWith("Remove"))
                {
                    method = "HttpDelete";
                    urlpath = item.Name.Replace("Delete", null).Replace("Remove", null);
                }
                else if (item.Name.StartsWith("Get") || item.Name.StartsWith("Load"))
                {
                    method = "HttpGet";
                    urlpath = item
                        .Name
                        .Replace("GetListWith", null)
                        .Replace("GetList", null)
                        .Replace("GetWith", null)
                        .Replace("Get", null)
                        .Replace("Load", null);
                }

                if (urlpath != null)
                {
                    urlpath = urlpath
                        .Replace("With", null)
                        .Replace("Async", null);
                    urlpath = RenderHelperFunctions.ToPluralize(urlpath);
                    urlpath = RenderHelperFunctions.ToSlugString(urlpath);
                }

                if (item.Params?.Any() == true)
                {
                    List<string> nps = new List<string>();
                    foreach (var p in item.Params)
                    {
                        if (p.Name.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!string.IsNullOrEmpty(urlpath))
                                urlpath += "/";
                            urlpath += "{" + RenderHelperFunctions.CamelCase(p.Name) + "}";
                        }
                    }
                }

                if (string.IsNullOrEmpty(urlpath))
                    urlpath = null;

                result[item.Name] = new AppServiceRouteInfo(method, urlpath);
            }

            return result;
        }
    }
}
