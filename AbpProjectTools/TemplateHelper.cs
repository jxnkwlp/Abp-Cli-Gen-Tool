using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scriban;
using Scriban.Runtime;
using System;
using System.IO;
using System.Linq;

namespace AbpProjectTools
{
    public static class TemplateHelper
    {
        public static string RenderString(string content, object data = null)
        {
            var scriptObject1 = new ScriptObject();
            scriptObject1.Import(typeof(RenderFunctions));
            scriptObject1.Import(data);

            var context = new TemplateContext();
            context.PushGlobal(scriptObject1);

            var template = Template.Parse(content);
            if (template.HasErrors)
            {
                var msg = template.Messages[0];
                throw new AggregateException(template.Messages.Select(x => new Exception(x.ToString())));
            }
            return template.Render(context);
        }

        public static string RenderStringFromFile(string file, object data = null)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(file);

            return RenderString(File.ReadAllText(file), data);
        }
    }

    public static class RenderFunctions
    {
        public static string Json(object value)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy(),
                },
            };

            return JsonConvert.SerializeObject(value, settings);
        }

        public static string Replace(string source, string match, string replaceTo)
        {
            return source.Replace(match, replaceTo);
        }

        public static string CamelCase(string source)
        {
            if (source == null)
                return source;

            return source[0].ToString().ToLower() + source.Substring(1);
        }
    }
}
