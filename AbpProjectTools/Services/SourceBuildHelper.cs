using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace AbpProjectTools.Services;

public static class SourceBuildHelper
{
    public static bool Build(string folder)
    {
        string argumentPrefix;
        string fileName;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            argumentPrefix = "-c";
            fileName = "/bin/bash";
        }
        else
        {
            argumentPrefix = "/C";
            fileName = "cmd.exe";
        }

        var procStartInfo = new ProcessStartInfo(fileName,
            $"{argumentPrefix} \" dotnet build \"{folder}\" \""
        );

        procStartInfo.CreateNoWindow = false;
        procStartInfo.UseShellExecute = false;

        bool hasError = false;

        try
        {
            var process = Process.Start(procStartInfo);
            process.ErrorDataReceived += (e, s) =>
            {
                hasError = true;
            };
            process.WaitForExit();

            Thread.Sleep(500);

            return hasError;
        }
        catch (Exception)
        {
            throw new Exception("Couldn't run Dotnet CLI...");
        }
    }
}
