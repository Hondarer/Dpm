// C#で自己登録型のWindows サービスアプリケーションを作成する
// https://symfoware.blog.fc2.com/blog-entry-1133.html

using Hondarersoft.Dpm.ServiceProcess;
using System;
using System.Reflection;
using System.Threading;

namespace Hondarersoft.Dpm
{
    public class EmptyService : ServiceBase
    {
        public static int Main(string[] args)
        {
            EmptyService instance = new EmptyService();

            ServiceInstallParameter serviceInstallParameter = new ServiceInstallParameter
            {
                ServiceBaseName = nameof(EmptyService),
                DisplayName = "Empty Service",
                Description = "Description of Empty Service"

                //InstanceID="TEST" // 扱いが難しい。どうあるべきか。インストールのときも、引数で指定すればよさそう。
            };

            if (instance.TryInstall(serviceInstallParameter) == true)
            {
                return instance.ExitCode;
            }

            Run(instance);

            return instance.ExitCode;
        }

        public EmptyService() : base()
        {
            //AutoLog = false; // // The default is true.
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("OnStart");

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("OnStop");

            ExitCode = 0;

            base.OnStop();
        }

        protected override void OnPause()
        {
            EventLog.WriteEntry("OnPause");

            base.OnPause();
        }

        protected override void OnContinue()
        {
            EventLog.WriteEntry("OnContinue");

            base.OnContinue();
        }

        protected override void OnShutdown()
        {
            // Event log service has already stopped at this timing.
            //xEventLog.WriteEntry("OnShutdown");

            base.OnShutdown();
        }
    }
}
