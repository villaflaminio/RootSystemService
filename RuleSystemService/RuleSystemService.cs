using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using log4net;
using st.rulesystem.sdk.PipeComunication.Interfaces;
using st.rulesystem.sdk.PipeComunication.Server;
using st.rulesystem.sdk.PipeComunication.Utilities;

namespace st.rulesystemservice
{
    public partial class RuleSystemService : ServiceBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(RuleSystemService));

        // Enum to specify the different states that the service could assume.
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007
        }

        // Define the struct of service status.
        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        }
        // Import .dll
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);


        public RuleSystemService()
        {
            try
            {
                _logger.Debug("Enter in DBLoaderService constructor");
                InitializeComponent();
            }
            catch (Exception error)
            {
                _logger.Error(error);
            }
        }
        protected override void OnStart(string[] args)
        {
            ServiceController service = new ServiceController();
            try
            {
                // Logging method enter
                _logger.Debug("Enter method RuleSystemService.OnStart");

                IPipeServer _server = new NPServer("elis_pipe", 100);
                _server.Start();
                _logger.Info("Server started");

                _server.MessageReceivedEvent += (sender, argss) =>
                {
                    _logger.Info("Message received from client" + argss);
                    string clientID = argss.ClientId;
                    PipeMessage message = argss.Message as PipeMessage;
                    object responsePipeServer = new PipeMessage("echo_reply", message.Message);

                    _server.SendMessage(clientID, responsePipeServer);

                };

                _server.ClientDisconnectedEvent += (sender, argss) =>
                {
                    _logger.Info("Client disconnected " + argss.ClientId);
                };
                _server.ClientConnectedEvent += (sender, argss) =>
                {
                    _logger.Info("Client connected " + argss.ClientId);
                };

                ServiceStatus serviceStatus = new ServiceStatus();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(ServiceHandle, ref serviceStatus);

            }
            catch (Exception e)
            {
                Stop();
                _logger.Fatal(e);
            }

        }

        protected override void OnStop()
        {
            try
            {
                // Logging method enter
                _logger.Debug("Enter method RuleSystemService.OnStop");

                // Update the service state to Stop Pending.
                ServiceStatus serviceStatus = new ServiceStatus();
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
                serviceStatus.dwWaitHint = 100000;
                SetServiceStatus(ServiceHandle, ref serviceStatus);

                // Update the service state to Stopped.
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(ServiceHandle, ref serviceStatus);

            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

        }
    }
}
