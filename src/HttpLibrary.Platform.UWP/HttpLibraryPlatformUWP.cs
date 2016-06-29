using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Platform.UWP
{
    public class HttpLibraryPlatformUWP : IHttpLibraryPlatform
    {
        private DeviceInfoUWP devInfo = new DeviceInfoUWP();
        public IDeviceInfo DeviceInfo
        {
            get
            {
                return devInfo;
            }
        }

        private TimerUWP timer = new TimerUWP();
        public ITimer Timer
        {
            get
            {
                return timer;
            }
        }

        private HttpSettingsUWP httpSettings = new HttpSettingsUWP();

        public IHttpSettings HttpSettings
        {
            get
            {
                return httpSettings;
            }
        }
    }
}
