using System;
using System.Runtime.InteropServices;

namespace Hondarersoft.Dpm
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class PInvoke
    {
        /// <summary>
        /// 読み取り専用のファイルマッピングオブジェクトへのアクセスのタイプを表します。
        /// </summary>
        internal const int FILE_MAP_READ = 2;

        /// <summary>
        /// 名前付きのファイルマッピングオブジェクトを開きます。
        /// </summary>
        /// <param name="dwDesiredAccess">ファイルマッピングオブジェクトへのアクセスのタイプを指定します。</param>
        /// <param name="bInheritHandle">返されたハンドルを、プロセスを作成する際に新しいプロセスに継承させるかどうかを指定します。true を指定すると、新しいプロセスはそのハンドルを継承します。</param>
        /// <param name="lpName">開くべきファイルマッピングオブジェクトの名前を保持している文字列へのポインタを指定します。</param>
        /// <returns>
        /// 関数が成功すると、ファイルマッピングオブジェクトに関連する、既に開いているハンドルが返ります。
        /// 関数が失敗すると、<see cref="IntPtr.Zero"/> が返ります。
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenFileMapping(int dwDesiredAccess, bool bInheritHandle, string lpName);
    }
}
