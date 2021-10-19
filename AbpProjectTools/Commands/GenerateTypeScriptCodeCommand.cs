using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

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


            var helper = new SwaggerHelper();

            command.Handler = CommandHandler.Create<GenerateTypeScriptCodeCommandOptions>(async options =>
            {
                Console.WriteLine($"🚗 Staring generate typescript services and typing file...");
                Console.WriteLine();

                var apiInfo = await helper.LoadAsync(options.SwaggerUrl);

                var content1 = TemplateFileHelper.GetFileContent("TypeScriptServices", options.Templates);
                content1 = TemplateHelper.RenderString(content1, new
                {
                    Apis = apiInfo.Apis.ToList(),
                    Count = apiInfo.Apis.Count,
                    Url = options.SwaggerUrl,
                    debug = options.Debug,
                });


                var content2 = TemplateFileHelper.GetFileContent("TypeScriptTypes", options.Templates);
                content2 = TemplateHelper.RenderString(content2, new
                {
                    schames = apiInfo.Schames.ToList(),
                    Count = apiInfo.Schames.Count,
                    Url = options.SwaggerUrl,
                    debug = options.Debug,
                });

                var outpath = options.Output;
                if (!string.IsNullOrWhiteSpace(options.ProjectName))
                    outpath = Path.Combine(options.Output, options.ProjectName);

                FileWrite(Path.Combine(outpath, "services.ts"), content1, true);
                FileWrite(Path.Combine(outpath, "typings.d.ts"), content2, true);

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

        public bool Debug { get; set; }

        public bool SplitType { get; set; }

        public bool SplitService { get; set; } = true;
    }

}
