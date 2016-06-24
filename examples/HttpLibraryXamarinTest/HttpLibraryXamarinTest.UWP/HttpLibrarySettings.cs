using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpLibraryXamarinTest;
using HttpLibrary.Interop;
using HttpLibrary.Platform.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(HttpLibraryXamarinTest.UWP.HttpLibrarySettings))]

namespace HttpLibraryXamarinTest.UWP
{
    class HttpLibrarySettings : IHttpLibrarySettings
    {
        private HttpLibraryPlatformUWP httpLibraryPlatform = new HttpLibraryPlatformUWP();
        public IHttpLibraryPlatform HttpLibraryPlaform
        {
            get
            {
                return httpLibraryPlatform;
            }
        }
    }
}
