using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Platform.Android
{
    public class HttpLibraryPlatformAndroid : IHttpLibraryPlatform
    {
        private DeviceInfoAndroid devInfo = new DeviceInfoAndroid();
        public IDeviceInfo DeviceInfo
        {
            get
            {
                return devInfo;
            }
        }

        private TimerAndroid timer = new TimerAndroid();
        public ITimer Timer
        {
            get
            {
                return timer;
            }
        }

        private HttpSettingsAndroid httpSettings = new HttpSettingsAndroid();

        public IHttpSettings HttpSettings
        {
            get
            {
                return httpSettings;
            }
        }
    }
}
