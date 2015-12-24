using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Riz.Common.WPF
{
    public class TerminatableRunner
    {
        bool _completed;
        public bool Run(Action action, Func<bool> checkStatusFunc, long timeout, ThreadPriority priority)
        {
            _completed = false;
            ThreadStart ts = new ThreadStart(() => 
            {
                action();
                _completed = true;
            });

            Thread workerThread = new Thread(ts);
            workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Priority = ThreadPriority.Highest;
            workerThread.Start();
            DateTime start = DateTime.Now;

            bool keepWaiting = true;
            while (
                (workerThread.ThreadState != ThreadState.Aborted) && 
                (workerThread.ThreadState != ThreadState.Stopped) &&
                keepWaiting &&
                (DateTime.Now-start).TotalMilliseconds<timeout)
            {
                keepWaiting = checkStatusFunc();
            }

            //ubij thread ako je slucajno ziv
            workerThread.Abort();

            return _completed;
        }
    }
}
