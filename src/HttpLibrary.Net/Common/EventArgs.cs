// Author: Wayne Gu
// Created: 2016-6-20 14:00
// Project: HttpLibrary.Net
// License: MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Common
{
    /// <summary>
    /// Proxy authorization required event arguments.
    /// </summary>
    public class ProxyAuthorizationRequiredEventArgs : EventArgs
    {
        //Proxy uri
        public string ProxyUri
        {
            get;
            internal set;
        }

        //Proxy name
        public string ProxyName
        {
            get;
            internal set;
        }

        /// <summary>
        /// User name
        /// </summary>
        public string ProxyUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Password
        /// </summary>
        public string ProxyPassword
        {
            get;
            set;
        }
    }
}
