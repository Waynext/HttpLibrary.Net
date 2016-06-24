using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Interop
{
    public interface IHttpSettings
    {
        void SetRequestUserAgent(HttpWebRequest request, string userAgent);
    }
}
