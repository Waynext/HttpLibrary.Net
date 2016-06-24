using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpLibraryXamarinTest;
using HttpLibrary.Interop;
using HttpLibrary.Platform.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(HttpLibraryXamarinTest.iOS.HttpLibrarySettings))]

namespace HttpLibraryXamarinTest.iOS
{
    class HttpLibrarySettings : IHttpLibrarySettings
    {
        private HttpLibraryPlatformiOS httpLibraryPlatform = new HttpLibraryPlatformiOS();

        public HttpLibrarySettings()
        {
        }
        public IHttpLibraryPlatform HttpLibraryPlaform
        {
            get
            {
                return httpLibraryPlatform;
            }
        }
    }
}
