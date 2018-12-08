// C#で自己登録型のWindows サービスアプリケーションを作成する
// https://symfoware.blog.fc2.com/blog-entry-1133.html

using Hondarersoft.Dpm.ServiceProcess;

namespace Hondarersoft.Dpm
{
    public class EmptyService : ServiceBase
    {
        public static int Main(string[] args)
        {
            EmptyService instance = new EmptyService();

            ServiceInstallParameter serviceInstallParameter = new ServiceInstallParameter
            {
                DisplayName = "Empty Service",
                Description = "Description of Empty Service"
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
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ExitCode = 0;

            base.OnStop();
        }
    }
}
