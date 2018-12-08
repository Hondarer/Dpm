// 参考:
// C#で自己登録型のWindows サービスアプリケーションを作成する
// https://symfoware.blog.fc2.com/blog-entry-1133.html

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public abstract class ServiceBase : System.ServiceProcess.ServiceBase
    {
        string[] Args { get; set; }

        public ServiceBase()
        {
            List<string> argsList = Environment.GetCommandLineArgs().ToList();
            argsList.RemoveAt(0);
            Args = argsList.ToArray();

            // TODO: コマンドライン引数の /InstanceID オプションを解釈して、ServiceName に設定する
            ServiceName = GetType().Name;
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
                if (Apis.IsServiceExists(serviceInstallParameter.ServiceName) == true)
                {
                    Console.Error.WriteLine("すでにインストールされています。");
                    ExitCode = 1;
                }
                else
                {
                    try
                    {
                        new IntegratedServiceInstaller().Install(serviceInstallParameter);

                        if (Apis.IsServiceExists(serviceInstallParameter.ServiceName) == true)
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
                if (Apis.IsServiceExists(serviceInstallParameter.ServiceName) != true)
                {
                    Console.Error.WriteLine("サービスがインストールされていません。");
                    ExitCode = 1;
                }
                else
                {
                    try
                    {
                        new IntegratedServiceInstaller().Uninstall(serviceInstallParameter);
                        if (Apis.IsServiceExists(serviceInstallParameter.ServiceName) != true)
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
