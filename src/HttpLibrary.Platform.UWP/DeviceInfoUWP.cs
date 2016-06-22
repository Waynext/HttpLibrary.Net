using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace HttpLibrary.Platform.UWP
{
    class DeviceInfoUWP : IDeviceInfo
    {
        private string appName;
        public string Application
        {
            get
            {
                var version = string.Format("{0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);
                return string.IsNullOrEmpty(appName) ? (string.Format("{0}/{1}", Package.Current.DisplayName, version)) : appName;
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
                EasClientDeviceInformation eas = new EasClientDeviceInformation();
                var deviceManufacturer = eas.SystemManufacturer;
                var deviceModel = eas.SystemProductName;

                return string.Format("{0} {1}", deviceManufacturer, deviceModel);
            }
        }

        public string System
        {
            get
            {
                AnalyticsVersionInfo ai = AnalyticsInfo.VersionInfo;
                var systemFamily = ai.DeviceFamily;

                // get the system version number
                //string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;

                // get the package architecure
                Package package = Package.Current;
                var systemArchitecture = package.Id.Architecture.ToString();

                return string.Format("{0}; {1}", systemFamily, systemArchitecture);
            }
        }
    }
}
