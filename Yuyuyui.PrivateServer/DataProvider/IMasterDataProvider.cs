using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public interface IMasterDataProvider
{
    public static IMasterDataProvider? ActiveProvider { get; set; } = null;

    string Directory { get; }
    Task Initialize();
}