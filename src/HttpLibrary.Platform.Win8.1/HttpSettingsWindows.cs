﻿using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace HttpLibrary.Platform.Windows
{
    class HttpSettingsWindows : IHttpSettings
    {
        public void SetRequestUserAgent(HttpWebRequest request, string userAgent)
        {
            //Not supported
            //request.UserAgent = userAgent;
        }
    }
}