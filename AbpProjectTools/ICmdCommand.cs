using System.CommandLine;

namespace AbpProjectTools
{
    public interface ICmdCommand
    {
        Command GetCommand();
    }
}
