using System;
using System.Runtime.InteropServices;

namespace Hondarersoft.Dpm
{
    public static partial class PInvoke
    {
        /// <summary>
        /// 開いているハンドルを閉じます。
        /// </summary>
        /// <param name="hObject">オブジェクトのハンドル。</param>
        /// <returns>
        /// 処理が成功すると、<c>true</c> を返します。処理が失敗すると、<c>false</c> を返します。
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);
    }
}
