using System.IO;

namespace AbpProjectTools
{
    public static class TemplateFileHelper
    {
        public static string GetFileContent(string name, string searchDirectory = null)
        {
            if (!string.IsNullOrEmpty(searchDirectory) && Directory.Exists(searchDirectory))
            {
                var filePath = Path.Combine(searchDirectory, $"{name}.sbn");
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
