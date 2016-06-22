using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLibrary.Interop
{
    public interface ITimer
    {
        void StartTimer(TimeSpan time, Func<bool> action);
    }
}
