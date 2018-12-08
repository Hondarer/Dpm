using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hondarersoft.Dpm.Areas
{
    public class CoTaskMemory : IDisposable
    {
        protected IntPtr intptr;

        public IntPtr Pointer

        {
            get
            {
                return intptr;
            }
        }

        public CoTaskMemory(int cb)
        {
            intptr = Marshal.AllocCoTaskMem(cb);
        }

        #region IDisposable

        /// <summary>
        /// リソースの解放が行われたかどうかを保持します。
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// リソースの解放が要求された際の処理を行います。
        /// </summary>
        public void Dispose()
        {
            // マネージドリソースの解放も依頼する
            Dispose(true);
        }

        /// <summary>
        /// オブジェクトがガベージ コレクタにより破棄される際の処理を行います。
        /// </summary>
        ~CoTaskMemory()
        {
            // マネージドリソースの解放は完了している
            Dispose(false);
        }

        /// <summary>
        /// リソースの解放の判定を行います。
        /// </summary>
        /// <param name="isDisposing"><see cref="IDisposable.Dispose"/> メソッドからの呼び出しかどうか。</param>
        private void Dispose(bool isDisposing)
        {
            // 多重解放を避ける
            lock (this)
            {
                if (disposed == false)
                {
                    // 未処理のため、リソース開放の実処理を実施
                    OnDispose(isDisposing);

                    disposed = true;

                    if (isDisposing == true)
                    {
                        // ファイナライザで行うべき操作を完了したので、ガベージコレクタに
                        // ファイナライザの呼び出しを抑制してもらう
                        GC.SuppressFinalize(this);
                    }
                }
            }
        }

        /// <summary>
        /// リソースの解放の実処理を行います。
        /// 本メソッドをオーバーライドする場合は、必ず基底クラスの <see cref="OnDispose(bool)"/> を呼び出してください。
        /// </summary>
        /// <param name="isDisposing">
        /// <see cref="Dispose()"/> メソッドからの呼び出しかどうか。
        /// true の場合は、マネージドリソースの解放が必要なことを示します。
        /// false の場合は、ガベージ コレクタによる呼び出しを示し、マネージドリソースの解放は不要です。
        /// </param>
        /// <example>
        /// この関数の一般的な実装方法を次の例に示します。
        /// <code><![CDATA[
        /// protected override void OnDispose(bool isDisposing)
        /// {
        ///     // 先にマネージドリソースを解放する
        ///     if (isDisposing == true)
        ///     {
        ///         // マネージドリソースの解放
        ///         if (this.マネージドリソース != null)
        ///         {
        ///             this.マネージドリソース.Dispose();
        ///             this.マネージドリソース = null;
        ///         }
        ///     }
        ///
        ///     // アンマネージドリソースの解放
        ///     if (this.アンマネージドリソース != IntPtr.Zero)
        ///     {
        ///         解放処理(this.マネージドリソース);
        ///         this.アンマネージドリソース = IntPtr.Zero;
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        protected virtual void OnDispose(bool isDisposing)
        {
            if (isDisposing == true)
            {
                // マネージドリソースの解放
                if (intptr != null)
                {
                    Marshal.FreeCoTaskMem(intptr);
                    intptr = IntPtr.Zero;
                }
            }
        }

        #endregion
    }
}
