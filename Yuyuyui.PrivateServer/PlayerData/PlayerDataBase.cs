using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Yuyuyui.PrivateServer;

public abstract class PlayerDataBase
{
    protected abstract string DataType { get; }

    [YamlIgnore]
    [JsonIgnore]
    public int _version = 0;
}