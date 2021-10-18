using System.IO;

namespace AbpProjectTools
{
    public static class TemplateFileHelper
    {
        public static string GetFileContent(string name, string directory = null)
        {
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                var filePath = Path.Combine(directory, $"{name}.sbn");
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }
            else
            {
                var filePath = $"./Tpl/{name}.sbn";
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }

            throw new System.Exception($"The template file '{name}' not found.");
        }
    }
}
