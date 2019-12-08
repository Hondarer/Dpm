using System;
using System.Runtime.InteropServices;

namespace Hondarersoft.Dpm
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class PInvoke
    {
        #region 定数

        /// <summary>
        /// 関数が成功したことを表します。
        /// </summary>
        internal const int MMSYSERR_NOERROR = 0;

        /// <summary>
        /// 関数が成功したことを表します。
        /// </summary>
        internal const int TIMERR_NOERROR = 0;

        #endregion

        #region 列挙

        /// <summary>
        /// flags for fuEvent parameter of timeSetEvent() function.
        /// </summary>
        internal enum TimerFlag
        {
            /// <summary>
            /// program timer for single event.
            /// </summary>
            OneShot = 0x0000,

            /// <summary>
            /// program for continuous periodic event.
            /// </summary>
            Periodic = 0x0001
        }

        #endregion

        #region API 宣言

        /// <summary>
        /// システム時刻をミリ秒単位で取得します。システム時刻は Windows が起動してから経過した時間です。
        /// </summary>
        /// <returns>システム時刻がミリ秒単位で返ります。</returns>
        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        internal static extern uint TimeGetTime();

        /// <summary>
        /// システム時刻をミリ秒単位で取得します。システム時刻は Windows が起動してから経過した時間です。この関数は timeGetTime 関数と同じように動作します。これらの関数の操作については、timeGetTime 関数を参照してください。
        /// </summary>
        /// <param name="pmmt">MMTIME 構造体のアドレスを指定します。</param>
        /// <param name="cbmmt">MMTIME 構造体のサイズをバイト単位で指定します。</param>
        /// <returns>関数が成功すると、TIMERR_NOERROR が返ります。システム時刻は、MMTIME 構造体の ms メンバに返されます。</returns>
        [DllImport("winmm.dll", EntryPoint = "timeGetSystemTime")]
        internal static extern int TimeGetSystemTime(out MMTIME pmmt, int cbmmt);

        /// <summary>
        /// 指定されたタイマイベントを開始します。
        /// </summary>
        /// <param name="uDelay">イベント遅延をミリ秒で指定します。この値が、タイマでサポートされるイベント遅延の最小値から最大値までの範囲にない場合、関数はエラーを返します。</param>
        /// <param name="uResolution">タイマイベントの分解能をミリ秒で指定します。値が小さいほど、分解能が増加します。分解能が 0 の場合は、最も可能な精度で周期イベントが発生します。ただし、システムのオーバーヘッドを減らすには、アプリケーションに適した最大値を使うようにしてください。</param>
        /// <param name="fptc">1 つのイベントの満了に対して 1 度、または周期イベントの満了に対して定期的に呼び出す、コールバック関数のアドレスを指定します。</param>
        /// <param name="dwUser">ユーザーが提供するコールバックデータを指定します。</param>
        /// <param name="fuEvent">タイマイベントのタイプを指定します。</param>
        /// <returns>タイマイベントの識別子が返ります。</returns>
        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        internal static extern int TimeSetEvent(int uDelay, int uResolution, MultimediaTimerCallback fptc, IntPtr dwUser, TimerFlag fuEvent);

        /// <summary>
        /// 指定されたタイマイベントを終了します。
        /// </summary>
        /// <param name="uTimerID">タイマイベントの識別子。</param>
        /// <returns>関数が成功すると、TIMERR_NOERROR が返ります。</returns>
        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        internal static extern int TimeKillEvent(int uTimerID);

        /// <summary>
        /// タイマデバイスを照会して、分解能を調べます。
        /// </summary>
        /// <param name="ptc">TIMECAPS 構造体のアドレスを指定します。この構造体には、タイマデバイスの分解能に関する情報が入ります。</param>
        /// <param name="cbtc">TIMECAPS 構造体のサイズをバイト単位で指定します。</param>
        /// <returns>関数が成功すると、MMSYSERR_NOERROR が返ります。</returns>
        [DllImport("winmm.dll", EntryPoint = "timeGetDevCaps")]
        internal static extern int TimeGetDevCaps(out TIMECAPS ptc, int cbtc);

        /// <summary>
        /// アプリケーションまたはデバイスドライバの最小タイマ分解能を設定します。
        /// </summary>
        /// <param name="uPeriod">アプリケーションまたはデバイスドライバの最小タイマ分解能を、ミリ秒単位で指定します。</param>
        /// <returns>関数が成功すると、TIMERR_NOERROR が返ります。uPeriod パラメータで指定した分解能が範囲外の場合、TIMERR_NOCANDO が返ります。</returns>
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        internal static extern int TimeBeginPeriod(int uPeriod);

        /// <summary>
        /// 以前にセットされた最小タイマ分解能をクリアします。
        /// </summary>
        /// <param name="uPeriod">timeBeginPeriod 関数への以前の呼び出しで指定された最小タイマ分解能を指定します。</param>
        /// <returns>関数が成功すると、TIMERR_NOERROR が返ります。uPeriod パラメータで指定した分解能が範囲外の場合、TIMERR_NOCANDO が返ります。</returns>
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        internal static extern int TimeEndPeriod(int uPeriod);

        #endregion

        #region 構造体

        /// <summary>
        /// The structure contains timing information for different types of multimedia data.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        internal struct MMTIME
        {
            /// <summary>
            /// indicates the contents of the union.
            /// </summary>
            [FieldOffset(0)]
            public uint wType;

            /// <summary>
            /// milliseconds.
            /// </summary>
            [FieldOffset(4)]
            public uint ms;

            /// <summary>
            /// samples.
            /// </summary>
            [FieldOffset(4)]
            public uint sample;

            /// <summary>
            /// byte count.
            /// </summary>
            [FieldOffset(4)]
            public uint cb;

            /// <summary>
            /// ticks in MIDI stream.
            /// </summary>
            [FieldOffset(4)]
            public uint ticks;

            /// <summary>
            /// SMPTE - hours.
            /// </summary>
            [FieldOffset(4)]
            public byte hour;

            /// <summary>
            /// SMPTE - minutes.
            /// </summary>
            [FieldOffset(5)]
            public byte min;

            /// <summary>
            /// SMPTE - seconds.
            /// </summary>
            [FieldOffset(6)]
            public byte sec;

            /// <summary>
            /// SMPTE - frames.
            /// </summary>
            [FieldOffset(7)]
            public byte frame;

            /// <summary>
            /// SMPTE - frames per second.
            /// </summary>
            [FieldOffset(8)]
            public byte fps;

            /// <summary>
            /// dummy.
            /// </summary>
            [FieldOffset(9)]
            public byte dummy;

            /// <summary>
            /// padding.
            /// </summary>
            [FieldOffset(10)]
            public byte pad0;

            /// <summary>
            /// padding.
            /// </summary>
            [FieldOffset(11)]
            public byte pad1;

            /// <summary>
            /// MIDI - song pointer position.
            /// </summary>
            [FieldOffset(4)]
            public uint songptrpos;
        }

        /// <summary>
        /// timer device capabilities data structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct TIMECAPS
        {
            /// <summary>
            /// minimum period supported.
            /// </summary>
            public int periodMin;

            /// <summary>
            /// maximum period supported.
            /// </summary>
            public int periodMax;
        }

        #endregion

        #region デリゲート

        /// <summary>
        /// マルチメディアタイマーからの呼び出しを処理するメソッドを表します。
        /// </summary>
        /// <param name="timerID">タイマーの識別子です。タイマーが複数起動されているとき、タイマーの識別に用います。</param>
        /// <param name="msg">予約。利用されません。</param>
        /// <param name="instance">timeSetEvent 関数の 4 番目で指定した引数です。</param>
        /// <param name="param1">予約。利用されません。</param>
        /// <param name="param2">予約。利用されません。</param>
        internal delegate void MultimediaTimerCallback(int timerID, int msg, IntPtr instance, IntPtr param1, IntPtr param2);

        #endregion
    }
}
