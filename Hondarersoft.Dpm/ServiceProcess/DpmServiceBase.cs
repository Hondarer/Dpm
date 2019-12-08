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
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;

namespace Hondarersoft.Dpm.ServiceProcess
{
    public class DpmServiceBase : ServiceBase
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

        public RemoteCommandSupports RemoteCommandSupport { get; protected set; } = RemoteCommandSupports.None;

        public const int UNSET_PORT_NUMBER = -1;

        public int TcpServicePort { get; protected set; } = UNSET_PORT_NUMBER;

        protected TcpChannel tcpServerChannel;
        protected IpcServerChannel ipcServerChannel;

        protected Func<RemoteCommandService> CreateRemoteCommandService { get; set; } = ()=>{ return new RemoteCommandService(); };

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
            CanShutdown = Apis.Reflection.IsMethodInherited(this, typeof(ServiceBase), nameof(OnShutdown));
            CanPauseAndContinue = (Apis.Reflection.IsMethodInherited(this, typeof(ServiceBase), nameof(OnPause)) || Apis.Reflection.IsMethodInherited(this, typeof(ServiceBase), nameof(OnContinue)));
        }

        /// <summary>
        /// サービス コントロール マネージャー (SCM) とサービスの実行可能ファイルを登録します。
        /// </summary>
        /// <param name="service"><see cref="ServiceBase"/> サービスを開始することを示します。</param>
        /// <exception cref="ArgumentException"><paramref name="service"/> は <c>null</c> です。</exception>
        public static new void Run(ServiceBase service)
        {
            if (Apis.ServiceProcess.IsServiceSession() == false)
            {
                Console.Error.WriteLine(Resources.Resource.NOT_A_SERVICE_SESSION);
                return;
            }

            ServiceBase.Run(service);
        }

        /// <summary>
        /// サービス コントロール マネージャー (SCM) に複数のサービス実行可能ファイルを登録します。
        /// </summary>
        /// <param name="services">サービスの開始を示す ServiceBase インスタンスの配列。</param>
        /// <exception cref="ArgumentException">サービスを開始するが指定されていません。 配列である可能性があります <c>null</c> または空です。</exception>
        public static new void Run(ServiceBase[] services)
        {
            if (Apis.ServiceProcess.IsServiceSession() == false)
            {
                Console.Error.WriteLine(Resources.Resource.NOT_A_SERVICE_SESSION);
                return;
            }

            ServiceBase.Run(services);
        }

        protected override void OnStart(string[] args)
        {
            if ((RemoteCommandSupport == RemoteCommandSupports.Ipc) || (RemoteCommandSupport == RemoteCommandSupports.Both))
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
            }

            RemoteCommandService remoteCommandService = null;
            if (RemoteCommandSupport != RemoteCommandSupports.None)
            {
                remoteCommandService = CreateRemoteCommandService();
                remoteCommandService.OnRemoteCommand += OnRemoteCommand;
            }

            if ((RemoteCommandSupport == RemoteCommandSupports.Tcp) || (RemoteCommandSupport == RemoteCommandSupports.Both))
            {
                if (TcpServicePort == UNSET_PORT_NUMBER)
                {
                    TcpServicePort = Apis.Remoting.GetRemoteTcpPort(this, remoteCommandService);
                }

                tcpServerChannel = new TcpChannel(TcpServicePort);
                ChannelServices.RegisterChannel(tcpServerChannel, false);
            }

            if (RemoteCommandSupport != RemoteCommandSupports.None)
            {
                RemotingServices.Marshal(remoteCommandService, remoteCommandService.GetType().Name);
            }

            if ((RemoteCommandSupport == RemoteCommandSupports.Tcp) || (RemoteCommandSupport == RemoteCommandSupports.Both))
            {
                tcpServerChannel.StartListening(null); // IPC の場合は、初回生成時は既に待ち受けを開始している
            }

            base.OnStart(args);
        }

#if false // 一時停止中もリモートオブジェクト自体は生かしておくほうがいいように思った。
        protected override void OnPause()
        {
            if ((RemoteCommandSupport == RemoteCommandSupports.Ipc) || (RemoteCommandSupport == RemoteCommandSupports.Both))
            {
                ipcServerChannel.StopListening(null);
            }
            if ((RemoteCommandSupport == RemoteCommandSupports.Tcp) || (RemoteCommandSupport == RemoteCommandSupports.Both))
            {
                tcpServerChannel.StopListening(null);
            }

            base.OnPause();
        }

        protected override void OnContinue()
        {
            if ((RemoteCommandSupport == RemoteCommandSupports.Ipc) || (RemoteCommandSupport == RemoteCommandSupports.Both))
            {
                ipcServerChannel.StartListening(null);
            }
            if ((RemoteCommandSupport == RemoteCommandSupports.Tcp) || (RemoteCommandSupport == RemoteCommandSupports.Both))
            {
                tcpServerChannel.StartListening(null);
            }
            base.OnContinue();
        }
#endif

        protected override void OnStop()
        {
            if ((RemoteCommandSupport == RemoteCommandSupports.Tcp) || (RemoteCommandSupport == RemoteCommandSupports.Both))
            {
                tcpServerChannel.StopListening(null);
            }

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

        public bool TryInstall(ServiceInstallParameter serviceInstallParameter = null)
        {
            if (serviceInstallParameter == null)
            {
                serviceInstallParameter = new ServiceInstallParameter();
            }

            if (Args.HasKey("ReInstall") == true)
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
            else if (Args.HasKey("Install") == true)
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
