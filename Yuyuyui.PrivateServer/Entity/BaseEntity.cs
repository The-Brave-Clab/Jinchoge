namespace Yuyuyui.PrivateServer
{
    public abstract class BaseEntity<T> : EntityBase 
        where T : BaseEntity<T>
    {
        public BaseEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }
    }
}