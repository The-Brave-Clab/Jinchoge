using System;

namespace Yuyuyui.AccountTransfer
{
    public abstract class BaseEntity<T> : EntityBase 
        where T : BaseEntity<T>
    {
        public BaseEntity(
            Uri requestUri,
            string httpMethod,
            RouteConfig config)
            : base(requestUri, httpMethod, config)
        {
        }
    }
}