using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Platform.Windows
{
    public class HttpLibraryPlatformWindows : IHttpLibraryPlatform
    {
        private DeviceInfoWindows devInfo = new DeviceInfoWindows();
        public IDeviceInfo DeviceInfo
        {
            get
            {
                return devInfo;
            }
        }

        private TimerWindows timer = new TimerWindows();
        public ITimer Timer
        {
            get
            {
                return timer;
            }
        }

        private HttpSettingsWindows httpSettings = new HttpSettingsWindows();
        public IHttpSettings HttpSettings
        {
            get
            {
                return httpSettings;
            }
        }
    }
}
