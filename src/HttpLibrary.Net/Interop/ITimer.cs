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
    /// Platform specific timer creator.
    /// </summary>
    public interface ITimer
    {
        void StartTimer(TimeSpan time, Func<bool> action);
    }
}
