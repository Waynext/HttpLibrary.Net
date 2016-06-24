using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Interop
{
    public interface IHttpLibraryPlatform
    {
        IDeviceInfo DeviceInfo
        {
            get;
        }

        ITimer Timer
        {
            get;
        }

        IHttpSettings HttpSettings
        {
            get;
        }
    }
}
