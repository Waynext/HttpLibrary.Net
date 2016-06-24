using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Platform.Win32
{
    public class HttpLibraryPlatformWin32 : IHttpLibraryPlatform
    {
        private DeviceInfoWin32 devInfo = new DeviceInfoWin32();
        public IDeviceInfo DeviceInfo
        {
            get
            {
                return devInfo;
            }
        }

        private TimerWin32 timer = new TimerWin32();
        public ITimer Timer
        {
            get
            {
                return timer;
            }
        }

        private HttpSettingsWin32 httpSettings = new HttpSettingsWin32();
        public IHttpSettings HttpSettings
        {
            get
            {
                return httpSettings;
            }
        }
    }
}
