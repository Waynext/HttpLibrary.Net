using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Common
{
    public class ProxyAuthorizationRequiredEventArgs : EventArgs
    {
        public string ProxyUri
        {
            get;
            set;
        }

        public string ProxyName
        {
            get;
            set;
        }

        public string ProxyUserName
        {
            get;
            set;
        }

        public string ProxyPassword
        {
            get;
            set;
        }
    }
}
