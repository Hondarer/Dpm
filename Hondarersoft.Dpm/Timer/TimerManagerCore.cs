using Hondarersoft.Dpm.ServiceProcess;
using System.Runtime.InteropServices;

namespace Hondarersoft.Dpm.Timer
{
    // TODO: PInvoke を直接コールせず、ラッパー API を作成すること。

    public class TimerManagerCore : DpmServiceBase
    {
        /// <summary>
        /// タイマ デバイスの最小分解能を変更している間、タイマ デバイスの最小分解能を保持します。
        /// </summary>
        private int? minPeriod = null;

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            if (PInvoke.TimeGetDevCaps(out PInvoke.TIMECAPS ptc, Marshal.SizeOf(typeof(PInvoke.TIMECAPS))) == PInvoke.MMSYSERR_NOERROR)
            {
                if (PInvoke.TimeBeginPeriod(ptc.periodMin) == PInvoke.TIMERR_NOERROR)
                {
                    EventLog.WriteEntry($"timeBeginPeriod done. uPeriod={ptc.periodMin}.");
                    minPeriod = ptc.periodMin;
                }
            }
        }

        protected override void OnStop()
        {
            if (minPeriod != null)
            {
                if (PInvoke.TimeEndPeriod((int)minPeriod) == PInvoke.TIMERR_NOERROR)
                {
                    EventLog.WriteEntry($"timeEndPeriod done. uPeriod={(int)minPeriod}.");
                    minPeriod = null;
                }
            }

            base.OnStop();
        }
    }
}
