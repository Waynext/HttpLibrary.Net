// Author: Wayne Gu
// Created: 2016-6-20 14:00
// Project: HttpLibrary.Net
// License: MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
namespace HttpLibrary.Requesting.Help
{
    class HttpHeaderHelper
    {
        public const string cookieKey = "Cookie";
        private static readonly char[] splitor = { ':' };
        private static readonly char[] splitor2 = { '=' };
        static public KeyValuePair<string, string>? SplitHeader(string header)
        {
            var headerParts = header.Split(splitor, StringSplitOptions.RemoveEmptyEntries);
            if (headerParts.Length == 2)
            {
                return new KeyValuePair<string, string>(headerParts[0].Trim(), headerParts[1].Trim());
            }

            return null;
        }

        static public bool IsHeaderCookie(string header)
        {
            return header.TrimStart().StartsWith(cookieKey);
        }

        static public Cookie CreateCookie(string cookie)
        {
            var cookieParts = cookie.Split(splitor2, StringSplitOptions.RemoveEmptyEntries);

            if (cookieParts.Length == 2)
            {
                return new Cookie(cookieParts[0], cookieParts[1]);
            }

            return null;
        }
    }
}
