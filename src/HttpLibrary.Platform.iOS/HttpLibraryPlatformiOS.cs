using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Platform.iOS
{
    public class HttpLibraryPlatformiOS : IHttpLibraryPlatform
    {
        private DeviceInfoiOS devInfo = new DeviceInfoiOS();
        public IDeviceInfo DeviceInfo
        {
            get
            {
                return devInfo;
            }
        }

        private TimeriOS timer = new TimeriOS();
        public ITimer Timer
        {
            get
            {
                return timer;
            }
        }

        private HttpSettingsiOS httpSettings = new HttpSettingsiOS();

        public IHttpSettings HttpSettings
        {
            get
            {
                return httpSettings;
            }
        }
    }
}
