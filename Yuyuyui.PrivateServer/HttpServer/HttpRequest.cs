namespace Yuyuyui.PrivateServer
{

    public class HttpRequest : RequestResponseBase
    {
        public HttpRequest() : base()
        {
            apiPath = "";
            httpMethod = "";
            httpVersion = "";
        }

        private string apiPath;
        public string ApiPath => apiPath;
        private string httpMethod;
        public string HttpMethod => httpMethod;
        private string httpVersion;
        public string HttpVersion => httpVersion;

        public void SetRequestDetail(string line)
        {
            var split = line.Split(' ', 3, StringSplitOptions.TrimEntries);
            httpMethod = split[0];
            apiPath = split[1];
            httpVersion = split[2];
        }
    }
}