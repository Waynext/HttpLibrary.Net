using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Interop
{
    public interface IDeviceInfo
    {
        string Device
        {
            get;
        }

        string System
        {
            get;
        }

        string Application
        {
            get;
            set;
        }
 
        
    }
}
