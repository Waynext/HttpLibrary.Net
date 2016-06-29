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
        /// Set http request header
        /// </summary>
        /// <param name="request">Http request</param>
        /// <param name="headerName">Header name</param>
        /// <param name="headerValue">Value</param>
        void SetHttpHeader(HttpWebRequest request, string headerName, string headerValue);
        void SetHttpHeader(HttpWebRequest request, HttpRequestHeader header, string headerValue);

    }
}
