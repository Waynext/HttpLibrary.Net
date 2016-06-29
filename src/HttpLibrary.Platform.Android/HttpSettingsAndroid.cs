using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace HttpLibrary.Platform.Android
{
    class HttpSettingsAndroid : IHttpSettings
    {
        public void SetHttpHeader(HttpWebRequest request, HttpRequestHeader header, string headerValue)
        {
            try
            {
                switch (header)
                {
                    case HttpRequestHeader.TransferEncoding:
                        request.TransferEncoding = headerValue;
                        break;
                    case HttpRequestHeader.UserAgent:
                        request.UserAgent = headerValue;
                        break;
                    case HttpRequestHeader.Accept:
                        request.Accept = headerValue;
                        break;
                    case HttpRequestHeader.Connection:
                        request.Connection = headerValue;
                        break;
                    case HttpRequestHeader.ContentLength:
                        request.ContentLength = long.Parse(headerValue);
                        break;
                    case HttpRequestHeader.ContentType:
                        request.ContentType = headerValue;
                        break;
                    case HttpRequestHeader.Date:
                        request.Date = DateTime.Parse(headerValue);
                        break;
                    case HttpRequestHeader.Expect:
                        request.Expect = headerValue;
                        break;
                    case HttpRequestHeader.Host:
                        request.Host = headerValue;
                        break;
                    case HttpRequestHeader.IfModifiedSince:
                        request.IfModifiedSince = DateTime.Parse(headerValue);
                        break;
                    case HttpRequestHeader.KeepAlive:
                        request.KeepAlive = bool.Parse(headerValue);
                        break;
                    case HttpRequestHeader.Referer:
                        request.Referer = headerValue;
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
