// Author: Wayne Gu
// Created: 2016-6-20 14:00
// Project: HttpLibrary.Net
// License: MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Interop
{
    /// <summary>
    /// Platform specific http property setter.
    /// </summary>
    public interface IHttpSettings
    {
        /// <summary>
        /// Set request user agent.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userAgent"></param>
        void SetRequestUserAgent(HttpWebRequest request, string userAgent);
    }
}
