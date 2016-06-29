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
                AssemblyName assemblyName = null;
                var assembly = Assembly.GetEntryAssembly();
                if(assembly != null)
                {
                    assemblyName = assembly.GetName();
                }
                
                if(assemblyName != null)
                    return string.IsNullOrEmpty(appName) ? (string.Format("{0}/{1}", assemblyName.Name, assemblyName.Version.ToString())) : appName;
                else
                {
                    return string.IsNullOrEmpty(appName) ? "App" : appName;
                }
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
