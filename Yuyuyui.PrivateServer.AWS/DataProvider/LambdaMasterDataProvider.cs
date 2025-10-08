using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Yuyuyui.PrivateServer.AWS;

// We use Lambda Layer to provide master data SQLite databases, so the files are always in /opt
public class LambdaMasterDataProvider : IMasterDataProvider
{
    public string Directory => "/opt";

    public Task Initialize()
    {
        // There's nothing to initialize
        return Task.CompletedTask;
    }
}