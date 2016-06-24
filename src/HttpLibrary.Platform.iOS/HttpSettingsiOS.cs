using HttpLibrary.Interop;
using System.Net;

namespace HttpLibrary.Platform.iOS
{
    class HttpSettingsiOS : IHttpSettings
    {
        public void SetRequestUserAgent(HttpWebRequest request, string userAgent)
        {
            request.UserAgent = userAgent;
        }
    }
}