// 参考:
// C#で自己登録型のWindows サービスアプリケーションを作成する
// https://symfoware.blog.fc2.com/blog-entry-1133.html

using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public abstract class InstallableServiceBase : ServiceBase
    {
        public string[] Args { get; private set; }

        public static new string ServiceName { get; private set; }

        public static string DisplayName { get; protected set; }

        public static string Description { get; protected set; }


        public InstallableServiceBase()
        {
            List<string> argsList = Environment.GetCommandLineArgs().ToList();
            argsList.RemoveAt(0);
            Args = argsList.ToArray();

            ServiceName = GetType().Name;
            base.ServiceName = ServiceName;
            DisplayName = ServiceName;
        }

        public bool TryInstall()
        {
            if (Args.Length < 1)
            {
                return false;
            }

            string mode = Args[0].ToLower();

            if (mode == "/install")
            {

                if (Apis.IsServiceExists(ServiceName) == true)
                {
                    Console.WriteLine("既にインストールされています。");

                }
                else
                {
                    string[] param = { System.Reflection.Assembly.GetEntryAssembly().Location };
                    ManagedInstallerClass.InstallHelper(param);
                }

                return true;
            }
            else if (mode == "/uninstall")
            {

                if (Apis.IsServiceExists(ServiceName) != true)
                {
                    Console.WriteLine("サービスがインストールされていません。");

                }
                else
                {

                    string[] param = { "/u", System.Reflection.Assembly.GetEntryAssembly().Location };
                    ManagedInstallerClass.InstallHelper(param);
                }

                return true;
            }

            return false;
        }
    }
}
