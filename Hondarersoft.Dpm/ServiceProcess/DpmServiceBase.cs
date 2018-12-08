// 参考:
// C#で自己登録型のWindows サービスアプリケーションを作成する
// https://symfoware.blog.fc2.com/blog-entry-1133.html

using Hondarersoft.Dpm.Environment;
using System;
using System.Reflection;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public abstract class DpmServiceBase : System.ServiceProcess.ServiceBase
    {
        public ProcessArgs Args { get; private set; }

        public string ServiceBaseName { get; private set; }

        public string InstanceID { get; private set; }

        public new string ServiceName
        {
            get
            {
                return base.ServiceName;
            }
            private set
            {
                base.ServiceName = value;
            }
        }

        public DpmServiceBase()
        {
            Args = new ProcessArgs(System.Environment.GetCommandLineArgs());

            Apis.Culture.ConfigureConsoleCulture();

            InstanceID = Args.GetValue("InstanceID");
            ServiceBaseName = GetType().Name;
            if (string.IsNullOrEmpty(InstanceID) != true)
            {
                ServiceName = string.Concat(ServiceBaseName, "_", InstanceID);
            }
            else
            {
                ServiceName = ServiceBaseName;
            }

            // シャットダウン可能、一時停止および再開可能を、派生クラスでのメソッド実装状態によって判定する。
            CanShutdown = IsMethodInherited(nameof(OnShutdown));
            CanPauseAndContinue = (IsMethodInherited(nameof(OnPause)) || IsMethodInherited(nameof(OnContinue)));
        }

        /// <summary>
        /// メソッドが派生クラスに実装されているかどうかを判定します。
        /// </summary>
        /// <param name="methodName">確認対象のメソッド名。</param>
        /// <returns>メソッドが派生クラスに実装されている場合は <c>true</c>、それ以外は <c>false</c>。</returns>
        protected bool IsMethodInherited(string methodName)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            // 当該メソッドが自分でないクラスに実装され、かつ、virturl である場合は true
            if ((method.DeclaringType != typeof(DpmServiceBase).BaseType) && (method.IsVirtual == true))
            {
                return true;
            }

            return false;
        }

        public bool TryInstall(ServiceInstallParameter serviceInstallParameter = null)
        {
            if (serviceInstallParameter == null)
            {
                serviceInstallParameter = new ServiceInstallParameter();
            }

            if (Args.HasKey("Install"))
            {
                if (Apis.ServiceProcess.IsServiceExists(ServiceName) == true)
                {
                    Console.Error.WriteLine(Resources.Resource.SERVICE_ALREADY_INSTALLED);
                    ExitCode = 1;
                }
                else
                {
                    try
                    {
                        new IntegratedServiceInstaller().Install(GetType().Name, InstanceID, serviceInstallParameter);

                        if (Apis.ServiceProcess.IsServiceExists(ServiceName) == true)
                        {
                            ExitCode = 0;
                        }
                        else
                        {
                            ExitCode = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                        ExitCode = 1;
                    }
                }

                return true;
            }
            else if (Args.HasKey("Uninstall"))
            {
                if (Apis.ServiceProcess.IsServiceExists(ServiceName) != true)
                {
                    Console.Error.WriteLine(Resources.Resource.SERVICE_ISNOT_INSTALLED);
                    ExitCode = 1;
                }
                else
                {
                    try
                    {
                        new IntegratedServiceInstaller().Uninstall(GetType().Name, InstanceID, serviceInstallParameter);
                        if (Apis.ServiceProcess.IsServiceExists(ServiceName) != true)
                        {
                            ExitCode = 0;
                        }
                        else
                        {
                            ExitCode = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                        ExitCode = 1;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
