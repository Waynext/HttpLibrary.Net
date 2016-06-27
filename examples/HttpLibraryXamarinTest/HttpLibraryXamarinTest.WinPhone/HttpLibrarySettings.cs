using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpLibraryXamarinTest;
using HttpLibrary.Interop;
using HttpLibrary.Platform.Windows;
using Xamarin.Forms;

[assembly: Dependency(typeof(HttpLibraryXamarinTest.WinPhone.HttpLibrarySettings))]

namespace HttpLibraryXamarinTest.WinPhone
{
    class HttpLibrarySettings : IHttpLibrarySettings
    {
        private HttpLibraryPlatformWindows httpLibraryPlatform = new HttpLibraryPlatformWindows();
        public IHttpLibraryPlatform HttpLibraryPlaform
        {
            get
            {
                return httpLibraryPlatform;
            }
        }
    }
}
