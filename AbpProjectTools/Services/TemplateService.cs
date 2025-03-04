﻿using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pluralize.NET;
using Scriban;
using Scriban.Runtime;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AbpProjectTools.Services;

public class TemplateService
{
    private readonly string _tempateDir;

    public TemplateService(string tempateDir = null)
    {
        _tempateDir = tempateDir;
    }

    private static string GetBaseDirectory()
    {
        return AppContext.BaseDirectory;
    }

    public static string GetFileContent(string fileName, string searchDirectory = null)
    {
        if (!string.IsNullOrEmpty(searchDirectory) && Directory.Exists(searchDirectory))
        {
            var filePath = Path.Combine(searchDirectory, $"{fileName}.sbn");
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
        }
        else
        {
            var filePath = Path.Combine(GetBaseDirectory(), $"./Tpl/{fileName}.sbn");
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
        }

        throw new Exception($"The template file '{fileName}' not found.");
    }

    public string Render(string fileName, object data = null)
    {
        var content = GetFileContent(fileName, _tempateDir);

        var scriptObject1 = new ScriptObject();
        scriptObject1.Import(typeof(RenderHelperFunctions));
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

}

public static class RenderHelperFunctions
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

    public static string ToSlugString(string source)
    {
        if (source == null)
            return source;

        StringBuilder sb = new StringBuilder();

        foreach (var item in source)
        {
            if (char.IsUpper(item))
            {
                if (sb.Length > 0)
                    sb.Append('-').Append(char.ToLower(item));
                else
                    sb.Append(char.ToLower(item));
            }
            else
            {
                sb.Append(item);
            }
        }

        return sb.ToString();
    }

    public static string ToPluralize(string source)
    {
        if (source == null)
            return source;

        return new Pluralizer().Pluralize(source);
    }

    public static string ToSingularize(string source)
    {
        if (source == null)
            return source;

        return new Pluralizer().Singularize(source);
    }

    public static string ToLower(string source)
    {
        if (source == null)
            return source;

        return source.ToLower();
    }

    public static string ToTitle(string source)
    {
        if (source == null)
            return source;

        return source.Humanize(LetterCasing.Title);
    }

    /// <summary>
    ///  "Pascal case input string is turned into sentence".Dehumanize() => "PascalCaseInputStringIsTurnedIntoSentence"
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToDehumanize(string source)
    {
        if (source == null)
            return source;

        return source.Dehumanize();
    }

    /// <summary>
    ///  "some_title for something".Pascalize() => "SomeTitleForSomething"
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToPascalize(string source)
    {
        if (source == null)
            return source;

        return source.Pascalize();
    }

    /// <summary>
    ///  "some_title for something".Camelize() => "someTitleForSomething"
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToCamelize(string source)
    {
        if (source == null)
            return source;

        return source.Camelize();
    }

    /// <summary>
    ///  "SomeText".Kebaberize() => "some-text"
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToKebaberize(string source)
    {
        if (source == null)
            return source;

        return source.Kebaberize();
    }
}
