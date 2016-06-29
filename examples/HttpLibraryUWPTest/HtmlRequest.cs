using HttpLibrary.Common;
using HttpLibrary.Http;
using HttpLibrary.Requesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibraryUWPTest
{
    class HtmlRequest : Request
    {
        public HtmlRequest(string uri) : base(MainPage.HttpLibPlatform, RequestPriority.Normal)
        {
            Uri = uri;
            
        }

        protected override void Encode()
        {
            HttpRequest = new HttpRequest(HttpLibPlatform, Uri, HttpConst.HttpMethod_Get);
            
        }
        protected override void CreateResponse()
        {
            Response = new HtmlResponse();
        }
    }

    class HtmlResponse : Response
    {
        public string Html
        {
            get;
            private set;
        }

        protected override void Decode()
        {
            var jsonResponse = HttpResponse as JSonResponse;
            if (jsonResponse == null)
            {
                ResponseStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ResponseStream, Encoding.UTF8))
                {
                    Html = reader.ReadToEnd();
                }
            }
            else
            {
                Html = jsonResponse.Object.ToString();
            }
        }
    }

}
