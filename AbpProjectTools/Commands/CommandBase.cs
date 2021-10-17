using System;
using System.CommandLine;
using System.IO;
using System.Linq;

namespace AbpProjectTools.Commands
{
    public abstract class CommandBase : ICmdCommand
    {
        public abstract Command GetCommand();

        protected static void FileWrite(string file, string content, bool overwite = false)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));

            if (File.Exists(file) && overwite == false)
                Console.WriteLine($"The file '{file}' exists.");
            else
                File.WriteAllText(file, content);
        }

        protected static string GetSolutionName(string dir)
        {
            var file = Directory.GetFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

            return Path.GetFileNameWithoutExtension(file);
        }

    }


}
