namespace Yuyuyui.PrivateServer
{
    public class RequestResponseBase
    {
        private Dictionary<string, string> headers;
        private byte[] body;

        public RequestResponseBase()
        {
            headers = new Dictionary<string, string>();
            body = Array.Empty<byte>();
        }

        public void AddHeaderFromLine(string line)
        {
            line = line.Trim();
            var split = line.Split(':', 2, StringSplitOptions.TrimEntries);
            headers.Add(split[0], split[1]);
        }

        public void SetBody(byte[] body)
        {
            this.body = body;
        }
    }
}