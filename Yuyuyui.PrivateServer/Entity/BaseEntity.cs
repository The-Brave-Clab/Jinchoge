using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace Yuyuyui.PrivateServer
{
    public class BaseEntity<T> : EntityBase 
        where T : BaseEntity<T>
    {
        public BaseEntity(SessionEventArgs e, Config config) : base(e, config)
        {
        }
    }
}