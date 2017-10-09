using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpLibraryXamarinTest;
using HttpLibrary.Interop;
using HttpLibrary.Platform.Android;
using Xamarin.Forms;
using Android.Content;

[assembly: Dependency(typeof(HttpLibraryXamarinTest.Droid.HttpLibrarySettings))]

namespace HttpLibraryXamarinTest.Droid
{
    class HttpLibrarySettings : IHttpLibrarySettings
    {
        private HttpLibraryPlatformAndroid httpLibraryPlatform = new HttpLibraryPlatformAndroid();

        public HttpLibrarySettings()
        {
            var appName = GetApplicationName(Forms.Context);
            if(!string.IsNullOrEmpty(appName))
            {
                HttpLibraryPlaform.DeviceInfo.Application = appName;
            }
        }
        public IHttpLibraryPlatform HttpLibraryPlaform
        {
            get
            {
                return httpLibraryPlatform;
            }
        }

        private static String GetApplicationName(Context context)
        {
            var appName = context.GetString(Resource.String.ApplicationName);
            var versionName = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            return appName + "/" + versionName;
        }
    }
}
