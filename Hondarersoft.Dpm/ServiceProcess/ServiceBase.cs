// 参考:
// C#で自己登録型のWindows サービスアプリケーションを作成する
// https://symfoware.blog.fc2.com/blog-entry-1133.html

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public abstract class ServiceBase : System.ServiceProcess.ServiceBase
    {
        string[] Args { get; set; }

        public ServiceBase()
        {
            List<string> argsList = System.Environment.GetCommandLineArgs().ToList();
            argsList.RemoveAt(0);
            Args = argsList.ToArray();

            Apis.Culture.ConfigureConsoleCulture();

            // TODO: コマンドライン引数の /InstanceID オプションを解釈して、ServiceName に設定する
            ServiceName = GetType().Name;

            // シャットダウン可能、一時停止および再開可能を、メソッドの実装状態によって判定する
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
            if ((method.DeclaringType != typeof(ServiceBase).BaseType) && (method.IsVirtual == true))
            {
                return true;
            }

            return false;
        }

        public bool TryInstall(ServiceInstallParameter serviceInstallParameter = null)
        {
            if (serviceInstallParameter == null)
            {
                serviceInstallParameter = new ServiceInstallParameter() { ServiceBaseName = GetType().Name };
            }

            if (Args.Length < 1)
            {
                return false;
            }

            string mode = Args[0].ToLower();

            if (mode == "/install")
            {
                if (Apis.ServiceProcess.IsServiceExists(serviceInstallParameter.ServiceName) == true)
                {
                    Console.Error.WriteLine("すでにインストールされています。");
                    ExitCode = 1;
                }
                else
                {
                    try
                    {
                        new IntegratedServiceInstaller().Install(serviceInstallParameter);

                        if (Apis.ServiceProcess.IsServiceExists(serviceInstallParameter.ServiceName) == true)
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
            else if (mode == "/uninstall")
            {
                if (Apis.ServiceProcess.IsServiceExists(serviceInstallParameter.ServiceName) != true)
                {
                    Console.Error.WriteLine("サービスがインストールされていません。");
                    ExitCode = 1;
                }
                else
                {
                    try
                    {
                        new IntegratedServiceInstaller().Uninstall(serviceInstallParameter);
                        if (Apis.ServiceProcess.IsServiceExists(serviceInstallParameter.ServiceName) != true)
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
