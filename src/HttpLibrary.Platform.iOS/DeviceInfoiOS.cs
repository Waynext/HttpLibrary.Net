using Foundation;
using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace HttpLibrary.Platform.iOS
{
    class DeviceInfoiOS : IDeviceInfo
    {
        public string Application
        {
            get
            {
                return string.Format("{0}/{1}", NSBundle.MainBundle.InfoDictionary["CFBundleDisplayName"] as NSString,
                              NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"] as NSString);
                
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public string Device
        {
            get
            {
                return UIDevice.CurrentDevice.Model;
            }
        }

        public string System
        {
            get
            {
                return string.Format("{0} {1}", UIDevice.CurrentDevice.SystemName, UIDevice.CurrentDevice.SystemVersion);
            }
        }
    }
}
