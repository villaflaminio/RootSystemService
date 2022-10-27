using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using st.rulesystem.sdk.logging;
using st.rulesystem.sdk.PipeComunication.Interfaces;
using st.rulesystem.sdk.PipeComunication.Utilities;

namespace st.rulesystem.sdk.PipeComunication.Server
{
    public class NPServer : IPipeServer
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(NPServer));

        private readonly string _pipeName;
        private readonly SynchronizationContext _synchronizationContext;
        private  IDictionary<string, InternalPipeServer> _servers; // ConcurrentDictionary is thread safe
        private int _maxNumberOfServerInstances = 1;

        public event EventHandler<ServerMessageReceivedEventArgs> MessageReceivedEvent;
        public event EventHandler<ClientConnectedEventArgs> ClientConnectedEvent;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnectedEvent;


        public NPServer(string pipeName, int MaxNumberOfServerInstances)
        {
            try
            {
                _logger.Debug("Enter in constructor of NPServer ");
                _logger.Trace(string.Format("Received parameters: pipeName: {0} , MaxNumberOfServerInstances : {1} ", pipeName, MaxNumberOfServerInstances));
                _pipeName = pipeName;
                _maxNumberOfServerInstances = MaxNumberOfServerInstances;
                _synchronizationContext = AsyncOperationManager.SynchronizationContext;
                _servers = new ConcurrentDictionary<string, InternalPipeServer>();
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }
        }

        #region ICommunicationServer implementation

        public string ServerId
        {
            get { return _pipeName; }
        }

        public void Start()
        {
            _logger.Debug("Start NPServer");
            StartNamedPipeServer();
        }

        public void Stop()
        {
            try
            {
                foreach (var server in _servers.Values)
                {
                    try
                    {
                        _logger.Debug("Stop of server " + server.ServerId);

                        UnregisterFromServerEvents(server);
                        server.Stop();
                    }
                    catch (Exception)
                    {
                        _logger.Error("Fialed to stop server " + server.ServerId);
                    }
                }

                _servers.Clear();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        #endregion

        #region event
        /// <summary>
        /// Fires MessageReceivedEvent in the current thread
        /// </summary>
        /// <param name="eventArgs"></param>
        private void OnMessageReceivedEvent(ServerMessageReceivedEventArgs eventArgs)
        {
            try
            {
                _logger.Info("Server "+ _pipeName + "-> New message recived : " + eventArgs.Message);
                _synchronizationContext.Post(e => MessageReceivedEvent.SafeInvoke(this, (ServerMessageReceivedEventArgs)e),
                                eventArgs);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

        }

        /// <summary>
        /// Fires ClientConnectedEvent in the current thread
        /// </summary>
        /// <param name="eventArgs"></param>
        private void OnClientConnectedEvent(ClientConnectedEventArgs eventArgs)
        {

            try
            {
                _logger.Info("Server " + _pipeName + " New Client connected " + eventArgs.ClientId);
                _synchronizationContext.Post(e => ClientConnectedEvent.SafeInvoke(this, (ClientConnectedEventArgs)e),
                                eventArgs);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        /// <summary>
        /// Fires ClientDisconnectedEvent in the current thread
        /// </summary>
        /// <param name="eventArgs"></param>
        private void OnClientDisconnectedEvent(ClientDisconnectedEventArgs eventArgs)
        {
            try
            {
                _logger.Info("Server " + _pipeName + " Client Disconnected : " + eventArgs.ClientId);
                _synchronizationContext.Post(
                               e => ClientDisconnectedEvent.SafeInvoke(this, (ClientDisconnectedEventArgs)e), eventArgs);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        /// <summary>
        /// Unregisters from the given server's events
        /// </summary>
        /// <param name="server"></param>
        private void UnregisterFromServerEvents(InternalPipeServer server)
        {
            _logger.Info("Server " + _pipeName + " UnregisterFromServerEvents ClientConnectedEvent , ClientDisconnectedEvent , MessageReceivedEvent ");
            server.ClientConnectedEvent -= ClientConnectedEventHandler;
            server.ClientDisconnectedEvent -= ClientDisconnectedEventHandler;
            server.MessageReceivedEvent -= MessageReceivedEventHandler;
        }

        #endregion

        #region event_handler
        /// <summary>
        /// Handles a client connection. Fires the relevant event and prepares for new connection.
        /// </summary>
        private void ClientConnectedEventHandler(object sender, ClientConnectedEventArgs eventArgs)
        {
            _logger.Debug("Enter in ClientConnectedEventHandler");
            OnClientConnectedEvent(eventArgs);
            if (_servers.Count < _maxNumberOfServerInstances) StartNamedPipeServer(); // Create a additional server as a preparation for new connection
        }

        /// <summary>
        /// Hanldes a client disconnection. Fires the relevant event ans removes its server from the pool
        /// </summary>
        private void ClientDisconnectedEventHandler(object sender, ClientDisconnectedEventArgs eventArgs)
        {
            _logger.Debug("Enter in ClientDisconnectedEventHandler");

            OnClientDisconnectedEvent(eventArgs);
            StopNamedPipeServer(eventArgs.ClientId);
        }

        /// <summary>
        /// Handles a message that is received from the client. Fires the relevant event.
        /// </summary>
        private void MessageReceivedEventHandler(object sender, ServerMessageReceivedEventArgs eventArgs)
        {
            _logger.Debug("Enter in MessageReceivedEventHandler");
            OnMessageReceivedEvent(eventArgs);
        }


        #endregion

        #region private methods

        /// <summary>
        /// Starts a new NamedPipeServerStream that waits for connection
        /// </summary>
        private void StartNamedPipeServer()
        {
            try
            {
                _logger.Debug("Enter in StartNamedPipeServer ");
                InternalPipeServer server = new InternalPipeServer(_pipeName, _maxNumberOfServerInstances);
                _servers[server.Id] = server;

                _logger.Trace("Subscribing to ClientConnectedEvent ,ClientDisconnectedEvent , MessageReceivedEvent ");
                server.ClientConnectedEvent += ClientConnectedEventHandler;
                server.ClientDisconnectedEvent += ClientDisconnectedEventHandler;
                server.MessageReceivedEvent += MessageReceivedEventHandler;

                server.Start();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        /// <summary>
        /// Stops the server that belongs to the given id
        /// </summary>
        /// <param name="id"></param>
        private void StopNamedPipeServer(string id)
        {
            try
            {
                _logger.Debug("Stop NPServer id : " + id);

                UnregisterFromServerEvents(_servers[id]);
                _servers[id].Stop();
                _servers.Remove(id);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }


        /// <summary>
        /// Starts a new NamedPipeServerStream that waits for connection
        /// </summary>
        public void SendMessage(string clientID, object message)
        {
            try
            {
                _logger.Debug("NPServer SendMessage to : " + clientID);
                Task<TaskResult> result;

                result = _servers.TryGetValue(clientID, out InternalPipeServer server) ? server.SendMessage(message) : throw new Exception("Client " + clientID + " not found");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

        }


        #endregion
    }
}
