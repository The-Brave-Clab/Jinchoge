using System;

namespace Yuyuyui.AccountTransfer
{
    public abstract class BaseEntity<T> : EntityBase 
        where T : BaseEntity<T>
    {
        public BaseEntity(
            Uri requestUri,
            string httpMethod,
            Config config)
            : base(requestUri, httpMethod, config)
        {
        }
    }
}