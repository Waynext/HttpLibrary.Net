using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Platform.Win32
{
    class DeviceInfoWin32 : IDeviceInfo
    {
        private string appName;
        public string Application
        {
            get
            {
                var assemblyName = Assembly.GetEntryAssembly().GetName();
                return string.IsNullOrEmpty(appName) ? (string.Format("{0}/{1}", assemblyName.Name, assemblyName.Version.ToString())) : appName;
            }

            set
            {
                appName = value;
            }
        }

        public string Device
        {
            get
            {
                return "PC";
            }
        }

        public string System
        {
            get
            {
                return Environment.OSVersion.ToString();
            }
        }
    }
}
