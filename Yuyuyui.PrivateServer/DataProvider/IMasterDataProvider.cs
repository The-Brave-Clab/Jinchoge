using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public interface IMasterDataProvider
{
    string Directory { get; }
    Task Initialize();
}