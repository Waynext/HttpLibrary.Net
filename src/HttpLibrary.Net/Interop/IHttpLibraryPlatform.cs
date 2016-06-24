// Author: Wayne Gu
// Created: 2016-6-20 14:00
// Project: HttpLibrary.Net
// License: MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Interop
{
    /// <summary>
    /// Platform specific interfaces factory
    /// </summary>
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
