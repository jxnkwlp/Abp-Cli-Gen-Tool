using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using AbpProjectTools.Models;
using AbpProjectTools.Services;

namespace AbpProjectTools.Commands;

public class FontendCrudCodeGeneratorCommand : CommandBase
{
    public override Command GetCommand()
    {
        var command = new Command("crud", "Generate crud page code base on swagger json document");

        command.AddOption(new Option<string>("--swagger-url", "The swagger api json document url") { IsRequired = true, });
        command.AddOption(new Option<string>(new[] { "--root", "-o" }, "") { IsRequired = true, });
        command.AddOption(new Option<string>("--name", "") { IsRequired = true, });
        command.AddOption(new Option<string>(new string[] { "--default-model", "-dm" }, ""));
        command.AddOption(new Option<string>(new[] { "--edit-model", "-em" }, ""));
        command.AddOption(new Option<string>("--templates", ""));
        command.AddOption(new Option<bool>("--overwrite", () => false));
        command.AddOption(new Option<bool>(new[] { "--gen-create-or-update", "-cu" }, () => true));


        var helper = new OpenApiDocumentService();

        command.Handler = CommandHandler.Create<FontendCrudCodeGeneratorCommandOptions>(async options =>
        {
            var templateService = new TemplateService(options.Templates);

            Console.WriteLine("🚗 Staring generate CRUD page code ...");
            Console.WriteLine($"🚗 Loading api document from url '{options.SwaggerUrl}'... ");

            var apiInfo = await helper.LoadAsync(options.SwaggerUrl);

            Console.WriteLine("🎉 Loading successful. ");

            var rootPath = options.Root;

            string defaultSchameName = options.DefaultModel ?? options.Name;
            string editSchameName = options.EditModel ?? options.Name;

            var defaultSchame = apiInfo.Schames.FirstOrDefault(x => x.Name.Equals(defaultSchameName, StringComparison.InvariantCulture));
            var editSchame = apiInfo.Schames.FirstOrDefault(x => x.Name.Equals(editSchameName, StringComparison.InvariantCulture));

            if (defaultSchame == null)
            {
                throw new Exception($"The name '{options.Name}' in schames not found.");
            }
            if (options.GenCreateOrUpdate && editSchame == null)
            {
                throw new Exception($"The model '{options.EditModel}' in schames not found.");
            }

            var defaultFields = GetSchameParams(defaultSchame, apiInfo);
            var editFields = GetSchameParams(editSchame, apiInfo);
            var allFields = defaultFields.Concat(editFields).Distinct().ToList();

            // crud
            var crudContent = templateService.Render("AntdCrud", new FontendCrudCodeGenerateOptions
            {
                Name = options.Name,
                DefaultModel = defaultSchameName,
                EditModel = editSchameName,
                AllFields = allFields,
                DefaultFields = defaultFields,
                EditFields = editFields,
                GenCreateOrUpdate = options.GenCreateOrUpdate,
            });
            WriteFileContent(Path.Combine(rootPath, "src", ".tmp", "pages", options.Name.ToLowerInvariant()) + ".tsx", crudContent, options.Overwrite);

            // locale
            var localeContent = templateService.Render("AntdCrudLocale", new FontendCrudCodeGenerateOptions
            {
                Name = options.Name,
                DefaultModel = defaultSchameName,
                EditModel = editSchameName,
                AllFields = allFields,
                DefaultFields = defaultFields,
                EditFields = editFields,
                GenCreateOrUpdate = options.GenCreateOrUpdate,
            });
            WriteFileContent(Path.Combine(rootPath, "src", ".tmp", "locales", $"pages.{options.Name.ToLowerInvariant()}.ts"), localeContent, options.Overwrite);

            Console.WriteLine("🎉 Done. ");
        });

        return command;
    }

    static List<ApiParamItem> GetSchameParams(ApiSchameDefinition definition, ApiInfoModel apiInfo)
    {
        var fields = definition.Params.ToList();
        fields = fields.Where(x => x.Name != "id" && x.Name != "extraProperties" && x.Name != "concurrencyStamp").ToList();

        void FillNestedObejct(ApiParamItem item)
        {
            if (item.Type == Models.ApiParamType.Object)
            {
                item.ObjectDefinition = apiInfo.Schames.FirstOrDefault(x => x.Name == item.ReferenceObjectName);
                if (item.ObjectDefinition != null)
                {
                    foreach (var field in item.ObjectDefinition.Params)
                    {
                        FillNestedObejct(field);
                    }
                }
            }
        }

        foreach (var field in fields)
        {
            FillNestedObejct(field);
        }

        return fields;
    }
}

public class FontendCrudCodeGeneratorCommandOptions
{
    public string SwaggerUrl { get; set; }
    public string Root { get; set; }
    public string Templates { get; set; }
    public string Name { get; set; }
    public string DefaultModel { get; set; }
    public string EditModel { get; set; }
    public bool Overwrite { get; set; }
    public bool GenCreateOrUpdate { get; set; }
}

public class FontendCrudCodeGenerateOptions
{
    public string Name { get; set; }
    public string DefaultModel { get; set; }
    public string EditModel { get; set; }
    public bool GenCreateOrUpdate { get; set; }

    public List<ApiParamItem> AllFields { get; set; }
    public List<ApiParamItem> DefaultFields { get; set; }
    public List<ApiParamItem> EditFields { get; set; }
}
