using System.IO;
using System.Linq;

namespace AbpProjectTools
{
    public class FileHelper
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

        public static DirectoryInfo GetWebProjectDirectory(string root)
        {
            var dir = new DirectoryInfo(root);

            return dir.EnumerateDirectories("*.Web", SearchOption.AllDirectories).FirstOrDefault();
        }

        public static DirectoryInfo GetDomainProjectDirectory(string root)
        {
            var dir = new DirectoryInfo(root);

            return dir.EnumerateDirectories("*.Domain", SearchOption.AllDirectories).FirstOrDefault();
        }

        public static DirectoryInfo GetHttpControllerProjectDirectory(string root)
        {
            var dir = new DirectoryInfo(root);

            return dir.EnumerateDirectories("*.HttpApi", SearchOption.AllDirectories).FirstOrDefault();
        }

        public static DirectoryInfo GetApplicationProjectDirectory(string root)
        {
            var dir = new DirectoryInfo(root);

            return dir.EnumerateDirectories("*.Application", SearchOption.AllDirectories).FirstOrDefault();
        }

        public static DirectoryInfo GetApplicationContractProjectDirectory(string root)
        {
            var dir = new DirectoryInfo(root);

            return dir.EnumerateDirectories("*.Application.Contracts", SearchOption.AllDirectories).FirstOrDefault();
        }

        public static DirectoryInfo GetEntityFrameworkCoreProjectDirectory(string root)
        {
            var dir = new DirectoryInfo(root);

            return dir.EnumerateDirectories("*.EntityFrameworkCore", SearchOption.AllDirectories).FirstOrDefault();
        }


    }
}
