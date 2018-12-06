// C#で自己登録型のWindows サービスアプリケーションを作成する
// https://symfoware.blog.fc2.com/blog-entry-1133.html

using Hondarersoft.Dpm.ServiceProcess;

namespace Hondarersoft.Dpm
{
    public class EmptyService : InstallableServiceBase
    {
        public static int Main(string[] args)
        {
            EmptyService instance = new EmptyService();

            if (instance.TryInstall() == true)
            {
                return 0;
            }

            Run(instance);

            return instance.ExitCode;
        }

        public EmptyService()
        {
            DisplayName = "Empty Service";
            Description = "Description of Empty Service";

            CanShutdown = true; // The default is false.
            CanPauseAndContinue = true; // The default is false.

            EventLog.WriteEntry(".ctor");
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
            EventLog.WriteEntry("OnShutdown");

            base.OnShutdown();
        }
    }
}
