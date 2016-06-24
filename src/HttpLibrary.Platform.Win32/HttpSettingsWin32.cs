using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace HttpLibrary.Platform.Win32
{
    class HttpSettingsWin32 : IHttpSettings
    {
        public void SetRequestUserAgent(HttpWebRequest request, string userAgent)
        {
            request.UserAgent = userAgent;
        }
    }
}
