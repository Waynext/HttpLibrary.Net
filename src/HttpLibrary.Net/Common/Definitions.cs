// Author: Wayne Gu
// Created: 2016-6-20 14:00
// Project: HttpLibrary.Net
// License: MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Common
{
    /// <summary>
    /// Request or response state
    /// </summary>
    public enum ReadyState
    {
        Succeeded,
        Failed,
        Cancelled
    }
}
