using System;
using System.CommandLine;
using System.IO;
using System.Linq;

namespace AbpProjectTools.Commands
{
    public abstract class CommandBase : ICmdCommand
    {
        public abstract Command GetCommand();

        protected static void FileWrite(string filePath, string content, bool overwite = false)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (File.Exists(filePath) && overwite == false)
            {
                Console.WriteLine($"The file '{filePath}' exists.");
            }
            else
            {
                File.WriteAllText(filePath, content);
                Console.WriteLine($"Write file '{filePath}' successful.");
            }
        }

        protected static string GetSolutionName(string dir)
        {
            var file = Directory.GetFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

            return Path.GetFileNameWithoutExtension(file);
        }

    }

}
