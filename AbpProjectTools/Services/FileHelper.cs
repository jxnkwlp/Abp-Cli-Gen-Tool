using System.IO;
using System.Linq;

namespace AbpProjectTools.Services;

public static class FileHelper
{
    public static FileInfo FindFile(string root, string name)
    {
        var dir = new DirectoryInfo(root);

        if (dir.Exists == false)
            throw new DirectoryNotFoundException(root);

        var file = dir.EnumerateFiles($"{name}.cs", SearchOption.AllDirectories).FirstOrDefault();

        if (file == null)
            throw new FileNotFoundException("File {name} not found.");

        return file;
    }

    public static DirectoryInfo GetHostProjectDirectory(string root)
    {
        var dir = new DirectoryInfo(root);

        var hostDir = dir.EnumerateDirectories("*.Web", SearchOption.AllDirectories).FirstOrDefault();

        if (hostDir == null)
            hostDir = dir.EnumerateDirectories("*.Web.Host", SearchOption.AllDirectories).FirstOrDefault();

        if (hostDir == null)
            hostDir = dir.EnumerateDirectories("*.Web.Unified", SearchOption.AllDirectories).FirstOrDefault();

        if (hostDir == null)
            hostDir = dir.EnumerateDirectories("*.HttpApi.Host", SearchOption.AllDirectories).FirstOrDefault();

        //if (hostDir == null)
        //    throw new System.Exception($"The host project not found in '{root}'. ");

        return hostDir;
    }

    public static DirectoryInfo GetDomainProjectDirectory(string root)
    {
        var dir = new DirectoryInfo(root);

        var dirs = dir.EnumerateDirectories("*.Domain", SearchOption.AllDirectories);
        return dirs.OrderBy(x => x.FullName.Length).FirstOrDefault();
    }

    public static DirectoryInfo GetDomainSharedProjectDirectory(string root)
    {
        var dir = new DirectoryInfo(root);

        var dirs = dir.EnumerateDirectories("*.Domain.Shared", SearchOption.AllDirectories);
        return dirs.OrderBy(x => x.FullName.Length).FirstOrDefault();
    }

    public static DirectoryInfo GetHttpControllerProjectDirectory(string root)
    {
        var dir = new DirectoryInfo(root);

        var dirs = dir.EnumerateDirectories("*.HttpApi", SearchOption.AllDirectories);
        return dirs.OrderBy(x => x.FullName.Length).FirstOrDefault();
    }

    public static DirectoryInfo GetApplicationProjectDirectory(string root)
    {
        var dir = new DirectoryInfo(root);

        var dirs = dir.EnumerateDirectories("*.Application", SearchOption.AllDirectories);
        return dirs.OrderBy(x => x.FullName.Length).FirstOrDefault();
    }

    public static DirectoryInfo GetApplicationContractProjectDirectory(string root)
    {
        var dir = new DirectoryInfo(root);

        var dirs = dir.EnumerateDirectories("*.Application.Contracts", SearchOption.AllDirectories);
        return dirs.OrderBy(x => x.FullName.Length).FirstOrDefault();
    }

    public static DirectoryInfo GetEntityFrameworkCoreProjectDirectory(string root)
    {
        var dir = new DirectoryInfo(root);

        var dirs = dir.EnumerateDirectories("*.EntityFrameworkCore", SearchOption.AllDirectories);
        return dirs.OrderBy(x => x.FullName.Length).FirstOrDefault();
    }

    public static DirectoryInfo GetMongoDBProjectDirectory(string root)
    {
        var dir = new DirectoryInfo(root);

        var dirs = dir.EnumerateDirectories("*.MongoDB", SearchOption.AllDirectories);
        return dirs.OrderBy(x => x.FullName.Length).FirstOrDefault();
    }
}
