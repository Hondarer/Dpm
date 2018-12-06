using System;
using System.Collections;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class ConfigurableServiceInstaller : ServiceInstaller
    {
        protected override void OnAfterInstall(IDictionary savedState)
        {
            //            サービスをインストールする際にWin32 API使って設定します。

            //ProjectInstaller 中で、ProjectInstaller_AfterInstall 内で行ないます。

            //Win32 API は、ChangeServiceConfig2 を使ってエラー時の再起動設定の変更

            //を行ないます。API を調べてみて下さい。

            Console.WriteLine("OnAfterInstall");

            base.OnAfterInstall(savedState);
        }
    }
}
