using Android.OS;
using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace HttpLibrary.Platform.Android
{
    class DeviceInfoAndroid : IDeviceInfo
    {
        private string appName = "App";
        public string Application
        {
            get
            {
                return appName;
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
                return Build.Model;
            }
        }

        public string System
        {
            get
            {
                return string.Format("Android {0}", Build.VERSION.Release);
            }
        }
    }
}
