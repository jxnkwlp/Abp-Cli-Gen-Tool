using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using AbpProjectTools.Models;

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
            command.AddOption(new Option<string[]>("--templates", ""));


            var helper = new SwaggerHelper();

            command.Handler = CommandHandler.Create<GenerateTypeScriptCodeCommandOptions>(async options =>
            {
                var apiInfo = await helper.LoadAsync(options.SwaggerUrl);

                var content = TemplateFileHelper.GetFileContent("TypeScriptServices", options.Templates);
                content = TemplateHelper.RenderString(content, new GenerateTypeScriptServicesCodeData
                {
                    Apis = apiInfo.Apis.ToList(),
                    Count = apiInfo.Apis.Count,
                    Url = options.SwaggerUrl
                });

                // Console.WriteLine(content);

                File.WriteAllText("test1.ts", content);
            });

            return command;
        }
    }

    public class GenerateTypeScriptCodeCommandOptions
    {
        public string SwaggerUrl { get; set; }
        public string Templates { get; set; }
        public string Output { get; set; }
        public string[] IgnoreUrls { get; set; }

    }

}
