﻿using System;
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
            var command = new Command("ts");

            command.AddOption(new Option<string>("--swagger-url", "The swagger api json document url") { IsRequired = true, });
            command.AddOption(new Option<string>(new[] { "--output", "-o" }, "") { IsRequired = true, });
            command.AddOption(new Option<string[]>("--ignore-urls", ""));
            command.AddOption(new Option<string>("--templates", ""));
            command.AddOption(new Option<string>("--project-name", ""));
            command.AddOption(new Option<bool>("--debug", ""));
            command.AddOption(new Option<bool>("--split-service", () => true));
            command.AddOption(new Option<bool>("--split-type", ""));
            command.AddOption(new Option<string[]>("--tags", ""));


            var helper = new SwaggerService();

            command.Handler = CommandHandler.Create<GenerateTypeScriptCodeCommandOptions>(async options =>
            {
                var templateService = new TemplateService(options.Templates);

                Console.WriteLine($"🚗 Staring generate typescript services and typing file...");
                Console.WriteLine($"🚗 Loading swagger document api url '{options.SwaggerUrl}'... ");

                var apiInfo = await helper.LoadAsync(options.SwaggerUrl);

                Console.WriteLine($"🎉 Loading successful. ");

                var outputPath = options.Output;
                if (!string.IsNullOrWhiteSpace(options.ProjectName))
                    outputPath = Path.Combine(options.Output, options.ProjectName);

                if (options.SplitService)
                {
                    foreach (var item in apiInfo.Apis.GroupBy(x => x.Tags[0]))
                    {
                        if (options.Tags?.Any() == true && !options.Tags.Contains(item.Key))
                        {
                            continue;
                        }

                        var name = item.Key;
                        var apiList = item.ToList();

                        var fileContent = templateService.Render("TypeScriptServices", new
                        {
                            Name = name,
                            Apis = apiList,
                            Count = apiList.Count(),
                            Url = options.SwaggerUrl,
                            Debug = options.Debug,
                            ProjectName = options.ProjectName,
                        });

                        WriteFileContent(Path.Combine(outputPath, $"{name}.ts"), fileContent, true);
                    }
                }
                else
                {
                    var fileContent = templateService.Render("TypeScriptService", new
                    {
                        Apis = apiInfo.Apis.ToList(),
                        Count = apiInfo.Apis.Count,
                        Url = options.SwaggerUrl,
                        Debug = options.Debug,
                        ProjectName = options.ProjectName,
                    });

                    WriteFileContent(Path.Combine(outputPath, "services.ts"), fileContent, true);
                }


                if (options.SplitType)
                {
                    foreach (var item in apiInfo.Schames)
                    {
                        var name = item.Name;

                        var fileContent = templateService.Render("TypeScriptType", new
                        {
                            schame = item,
                            Url = options.SwaggerUrl,
                            Debug = options.Debug,
                            ProjectName = options.ProjectName,
                        });

                        WriteFileContent(Path.Combine(outputPath, $"{name}.typings.d.ts"), fileContent, true);
                    }
                }
                else
                {
                    var fileContent = templateService.Render("TypeScriptTypes", new
                    {
                        schames = apiInfo.Schames.ToList(),
                        Count = apiInfo.Schames.Count,
                        Url = options.SwaggerUrl,
                        Debug = options.Debug,
                        ProjectName = options.ProjectName,
                    });

                    WriteFileContent(Path.Combine(outputPath, "typings.d.ts"), fileContent, true);
                }

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

        public bool SplitType { get; set; }

        public bool SplitService { get; set; } = true;
    }

}
