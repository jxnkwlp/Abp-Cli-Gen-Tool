﻿using Pastel;
using System;
using System.CommandLine;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace AbpProjectTools.Commands;

public abstract class CommandBase : ICmdCommand
{
    public abstract Command GetCommand();

    protected static void WriteFileContent(string filePath, string content, bool overwrite = false)
    {
        var directory = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        //#if DEBUG
        //        Console.WriteLine($"⬇️⬇️ File '{filePath}' Content Preview ... ".Pastel(Color.Aqua));
        //        Console.WriteLine(content.Pastel(Color.Aqua));
        //        Console.WriteLine();
        //#endif

        if (File.Exists(filePath) && overwrite == false)
        {
            Console.WriteLine($"➡️ The file '{filePath}' exists.".Pastel(Color.Yellow));
        }
        else
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
            Console.WriteLine($"⬇️ Write file '{filePath}' successful.".Pastel(Color.Green));
        }
    }

    protected static string GetSolutionName(string dir)
    {
        var file = Directory.GetFiles(dir, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();

        return Path.GetFileNameWithoutExtension(file);
    }

    protected static string GetDefaultSolutionName()
    {
        var slnFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln", SearchOption.TopDirectoryOnly);

        if (slnFiles?.Length == 1)
        {
            var name = Path.GetFileNameWithoutExtension(slnFiles[0]);

            if (name.LastIndexOf('.') > 0)
            {
                name = name.Substring(name.LastIndexOf('.') + 1);
            }

            return name;
        }

        return null;
    }
}
