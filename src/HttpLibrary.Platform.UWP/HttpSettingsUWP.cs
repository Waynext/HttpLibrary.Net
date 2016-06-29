using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace HttpLibrary.Platform.UWP
{
    class HttpSettingsUWP : IHttpSettings
    {
        public void SetHttpHeader(HttpWebRequest request, HttpRequestHeader header, string headerValue)
        {
            try
            {
                switch (header)
                {
                    case HttpRequestHeader.Accept:
                        request.Accept = headerValue;
                        break;
                    case HttpRequestHeader.ContentType:
                        request.ContentType = headerValue;
                        break;
                    default:
                        request.Headers[header] = headerValue;
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception throw when set http header " + header + "=" + headerValue);
                Debug.WriteLine(ex);
                throw;
            }

        }

        public void SetHttpHeader(HttpWebRequest request, string headerName, string headerValue)
        {
            try
            {
                request.Headers[headerName] = headerValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception throw when set http header " + headerName + "=" + headerValue);
                Debug.WriteLine(ex);
                throw;
            }
        }
    }
}
