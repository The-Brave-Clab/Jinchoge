using System;
using System.Collections.Generic;

namespace Yuyuyui.PrivateServer;

public struct EventArgs
{
    public Uri requestUri;
    public string requestMethod;
    public Dictionary<string, string> header;
    public byte[] requestBody;
}