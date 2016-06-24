using HttpLibrary.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpLibrary.Platform.iOS
{
    class TimeriOS : ITimer
    {
        private Timer timer;

        private Func<bool> actionFunc;
        public void StartTimer(TimeSpan time, Func<bool> action)
        {
            actionFunc = action;
            timer = new Timer(TimerProcess, null, TimeSpan.FromSeconds(0), time);
        }

        private void TimerProcess(object state)
        {
            if (!actionFunc())
            {
                timer.Dispose();
            }
        }
    }
}
