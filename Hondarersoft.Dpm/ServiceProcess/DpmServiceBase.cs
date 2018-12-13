// 参考:
// C#で自己登録型のWindows サービスアプリケーションを作成する
// https://symfoware.blog.fc2.com/blog-entry-1133.html

using Hondarersoft.Dpm.Environment;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class DpmServiceBase : System.ServiceProcess.ServiceBase
    {
        public ProcessArgs Args { get; private set; }

        private string serviceBaseName = null;

        public string ServiceBaseName
        {
            get
            {
                return serviceBaseName;
            }
            protected set
            {
                serviceBaseName = value;

                if ((SupportMultiInstance == true) && (string.IsNullOrEmpty(InstanceID) != true))
                {
                    ServiceName = string.Concat(serviceBaseName, "_", InstanceID);
                }
                else
                {
                    ServiceName = serviceBaseName;
                }
            }
        }

        private bool supportMultiInstance = false;

        public bool SupportMultiInstance
        {
            get
            {
                return supportMultiInstance;
            }
            protected set
            {
                supportMultiInstance = value;

                if (supportMultiInstance == false)
                {
                    ServiceName = ServiceBaseName;
                }
                else if (string.IsNullOrEmpty(InstanceID) != true)
                {
                    ServiceName = string.Concat(ServiceBaseName, "_", InstanceID);
                }
            }
        }

        private string instanceID = null;

        public string InstanceID
        {
            get
            {
                if (supportMultiInstance == false)
                {
                    return null;
                }
                return instanceID;
            }
            protected set
            {
                instanceID = value;

                if (supportMultiInstance == true)
                {
                    if (string.IsNullOrEmpty(instanceID) != true)
                    {
                        ServiceName = string.Concat(ServiceBaseName, "_", instanceID);
                    }
                }
            }
        }

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

        // TODO: IPC と TCP の共存は試していない

        public RemoteCommandSupports RemoteCommandSupport { get; protected set; } = RemoteCommandSupports.None;

        public int TcpServicePort { get; protected set; } = 0;

        protected IpcServerChannel ipcServerChannel;

        public DpmServiceBase()
        {
            Args = new ProcessArgs(System.Environment.GetCommandLineArgs());

            Apis.Culture.ConfigureConsoleCulture();

            InstanceID = Args.GetValue("InstanceID");
            ServiceBaseName = GetType().Name;

            //CanStop = false; // The default is true.
            //AutoLog = false; // The default is true.
            //SupportInstanceID = true; // The default is false.

            // シャットダウン可能、一時停止および再開可能を、派生クラスでのメソッド実装状態によって判定する。
            CanShutdown = IsMethodInherited(nameof(OnShutdown));
            CanPauseAndContinue = (IsMethodInherited(nameof(OnPause)) || IsMethodInherited(nameof(OnContinue)));
        }

        protected Func<RemoteCommandService> CreateRemoteCommandService { get; set; } = (()=>{ return new RemoteCommandService(); });

        protected override void OnStart(string[] args)
        {
            if (RemoteCommandSupport == RemoteCommandSupports.Ipc)
            {
                //// Local administrators sid
                //SecurityIdentifier localAdminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

                //// Local Power users sid
                //SecurityIdentifier powerUsersSid = new SecurityIdentifier(WellKnownSidType.BuiltinPowerUsersSid, null);

                // Everyone users sid
                SecurityIdentifier everoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                //// Network sid
                //SecurityIdentifier networkSid = new SecurityIdentifier(@"S-1-5-2");

                DiscretionaryAcl dacl = new DiscretionaryAcl(false, false, 1);

                //// Disallow access
                //dacl.AddAccess(AccessControlType.Deny, networkSid, -1, InheritanceFlags.None, PropagationFlags.None);

                //// Allow acces
                //dacl.AddAccess(AccessControlType.Allow, localAdminSid, -1, InheritanceFlags.None, PropagationFlags.None);
                //dacl.AddAccess(AccessControlType.Allow, powerUsersSid, -1, InheritanceFlags.None, PropagationFlags.None);
                dacl.AddAccess(AccessControlType.Allow, everoneSid, -1, InheritanceFlags.None, PropagationFlags.None);

                CommonSecurityDescriptor securityDescriptor =
                    new CommonSecurityDescriptor(false, false,
                        ControlFlags.GroupDefaulted |
                        ControlFlags.OwnerDefaulted |
                        ControlFlags.DiscretionaryAclPresent,
                        null, null, null, dacl);

                IDictionary props = new Hashtable
                {
                    ["name"] = ServiceName,
                    ["portName"] = ServiceName
                };

                ipcServerChannel = new IpcServerChannel(props, null, securityDescriptor);
                ChannelServices.RegisterChannel(ipcServerChannel, true);
                RemoteCommandService remoteCommandService = new RemoteCommandService();

                remoteCommandService.OnRemoteCommand += OnRemoteCommand;

                RemotingServices.Marshal(remoteCommandService, remoteCommandService.GetType().Name);
            }

            base.OnStart(args);
        }

        // TODO: Pause/Continue の際に、リモーティングサービスを停止させる

        protected override void OnStop()
        {
            // Set default exit code.
            // If another ExitCode is set in a derived class,
            // it is necessary to consider whether to call this method.
            ExitCode = 0;

            base.OnStop();
        }

        protected virtual object OnRemoteCommand(object sender, RemoteCommandEventArgs eventArgs)
        {
            return null;
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

            if (Args.HasKey("Install") == true)
            {
                if (Apis.Principal.IsAdministrator() != true)
                {
                    Console.Error.WriteLine("You are not Administrator.");
                    ExitCode = 1;
                }
                else if (Apis.ServiceProcess.IsServiceExists(ServiceName) == true)
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
            else if (Args.HasKey("Uninstall") == true)
            {
                if (Apis.Principal.IsAdministrator() != true)
                {
                    Console.Error.WriteLine("You are not Administrator.");
                    ExitCode = 1;
                }
                else if (Apis.ServiceProcess.IsServiceExists(ServiceName) != true)
                {
                    Console.Error.WriteLine(Resources.Resource.SERVICE_ISNOT_INSTALLED);
                    ExitCode = 1;
                }
                else
                {
                    try
                    {
                        new IntegratedServiceInstaller().Uninstall(GetType().Name, InstanceID);
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
