﻿using HttpLibrary.Common;
using HttpLibrary.Http;
using HttpLibrary.Requesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibraryWin32Test
{
    class HtmlRequest : Request
    {
        public HtmlRequest(string uri) : base(RequestPriority.Normal)
        {
            Uri = uri;
            
        }

        protected override void Encode()
        {
            HttpRequest = new HttpRequest(Uri, HttpConst.HttpMethod_Get);
            
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
            ResponseStream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(ResponseStream, Encoding.UTF8))
            {
                Html = reader.ReadToEnd();
            }
        }
    }

}