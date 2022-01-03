namespace Yuyuyui.PrivateServer
{
    public abstract class BaseEntity<T> : EntityBase 
        where T : BaseEntity<T>
    {
        public BaseEntity(
            Uri requestUri,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, requestHeaders, requestBody, config)
        {
        }
    }
}