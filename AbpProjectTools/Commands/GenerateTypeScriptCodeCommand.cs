using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using AbpProjectTools.Services;

namespace AbpProjectTools.Commands
{
    public class GenerateTypeScriptCodeCommand : CommandBase
    {
        public override Command GetCommand()
        {
            var command = new Command("ts", "Generate ts types and services code base on swagger json document");

            command.AddOption(new Option<string>("--swagger-url", "The swagger api json document url") { IsRequired = true, });
            command.AddOption(new Option<string>(new[] { "--output", "-o" }, "") { IsRequired = true, });
            command.AddOption(new Option<string[]>("--ignore-urls", ""));
            command.AddOption(new Option<string>("--templates", ""));
            command.AddOption(new Option<string>("--project-name", ""));
            command.AddOption(new Option<bool>("--debug", ""));
            command.AddOption(new Option<string[]>("--tags", ""));
            command.AddOption(new Option<string>("--request-import", ""));


            var helper = new OpenApiDocumentService();

            command.Handler = CommandHandler.Create<GenerateTypeScriptCodeCommandOptions>(async options =>
            {
                var templateService = new TemplateService(options.Templates);

                Console.WriteLine($"🚗 Staring generate typescript services and typing file...");
                Console.WriteLine($"🚗 Loading api document from url '{options.SwaggerUrl}'... ");

                var apiInfo = await helper.LoadAsync(options.SwaggerUrl);

                Console.WriteLine($"🎉 Loading successful. ");

                var outputPath = options.Output;
                if (!string.IsNullOrWhiteSpace(options.ProjectName))
                    outputPath = Path.Combine(options.Output, options.ProjectName);

                // service
                foreach (var item in apiInfo.Apis.GroupBy(x => x.Tags[0]))
                {
                    if (options.Tags?.Any() == true && !options.Tags.Contains(item.Key))
                    {
                        continue;
                    }

                    var name = item.Key;
                    var apiList = item.OrderBy(x => x.OperationId).ToList();

                    var fileContent = templateService.Render("TypeScriptServices", new
                    {
                        projectName = options.ProjectName,
                        Name = name,
                        Apis = apiList,
                        Count = apiList.Count(),
                        Url = options.SwaggerUrl,
                        Debug = options.Debug,
                        ProjectName = options.ProjectName,
                        RequestImport = options.RequestImport,
                    });

                    WriteFileContent(Path.Combine(outputPath, $"{name}.ts"), fileContent, true);
                }

                // types
                var fileContent2 = templateService.Render("TypeScriptTypes", new
                {
                    projectName = options.ProjectName,
                    schames = apiInfo.Schames.ToList(),
                    Count = apiInfo.Schames.Count,
                    Url = options.SwaggerUrl,
                    Debug = options.Debug,
                    ProjectName = options.ProjectName,

                });

                WriteFileContent(Path.Combine(outputPath, "typings.d.ts"), fileContent2, true);

                // enums
                fileContent2 = templateService.Render("TypeScriptEnums", new
                {
                    projectName = options.ProjectName,
                    schames = apiInfo.Schames.OrderBy(x => x.Name).ToList(),
                    Count = apiInfo.Schames.Count,
                    Url = options.SwaggerUrl,
                    Debug = options.Debug,
                    ProjectName = options.ProjectName,
                });

                WriteFileContent(Path.Combine(outputPath, "enums.ts"), fileContent2, true);

                Console.WriteLine("🎉 Done. ");
            });

            return command;
        }
    }

    public class GenerateTypeScriptCodeCommandOptions
    {
        public string SwaggerUrl { get; set; }
        public string Templates { get; set; }
        public string ProjectName { get; set; }
        public string Output { get; set; }
        public string[] IgnoreUrls { get; set; }
        public string[] Tags { get; set; }

        public bool Debug { get; set; }

        public string RequestImport { get; set; }
    }

}
