using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using st.rulesystem.sdk.logging;
using st.rulesystem.sdk.PipeComunication.Interfaces;
using st.rulesystem.sdk.PipeComunication.Utilities;

namespace st.rulesystem.sdk.PipeComunication.Client
{
    public class NPClient : IPipeClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(NPClient));

        private NamedPipeClientStream _pipeClient;
        public event EventHandler<ClientMessageReceivedEventArgs> MessageReceivedEvent;
        private readonly SynchronizationContext _synchronizationContext;
        public static BinaryFormatter BinaryFormatter = new BinaryFormatter();

        public NPClient(string pipeName)
        {
            try
            {
                _logger.Debug("Enter in constructor of NPClient ");
                _logger.Trace("Recived parameter  pipeName = " + pipeName);
                _synchronizationContext = AsyncOperationManager.SynchronizationContext;

                _pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            }
            catch (Exception e)
            {

                _logger.Fatal(e);
            }
        }


        #region ICommunicationClient implementation

        /// <summary>
        /// Starts the client. Connects to the server.
        /// </summary>
        public void Start()
        {
            try
            {
                _logger.Debug("Enter in Start method of NPClient ");

                DateTime start = DateTime.Now;

                const int tryConnectTimeout = 60 * 1000; // 1 minuto
                try
                {
                    _logger.Debug("Try to connect to server ");
                    _pipeClient.Connect(tryConnectTimeout);
                    _logger.Debug("Connected to server");

                }
                catch (Exception e)
                {
                    _logger.Fatal(e);
                }

                if (_pipeClient.IsConnected)
                {
                    BeginRead(new BufferReading());
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        /// <summary>
        /// Stops the client. Waits for pipe drain, closes and disposes it.
        /// </summary>
        public void Stop()
        {
            try
            {
                _logger.Debug("Enter in Stop method of NPClient ");
                _pipeClient.WaitForPipeDrain();
            }
            catch (Exception e)

            {
                _logger.Error(e);
            }
            finally
            {

                _pipeClient.Close();
                _pipeClient.Dispose();
            }
        }

        #endregion

        #region event

        /// <summary>
        /// This method fires MessageReceivedEvent with the given message
        /// </summary>
        private void OnMessageReceived(object message)
        {
            try
            {
                _logger.Info("New message recived : " + message);

                ClientMessageReceivedEventArgs args = new ClientMessageReceivedEventArgs { Message = message };
                _synchronizationContext.Post(e => MessageReceivedEvent.SafeInvoke(this, (ClientMessageReceivedEventArgs)e), args);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        #endregion

        #region  methods

        /// <summary>
        /// This method begins an asynchronous read operation.
        /// </summary>
        private void BeginRead(BufferReading bufferReading)
        {
            try
            {
                _logger.Debug("Enter in BeginRead method of NPClient ");

                _pipeClient.BeginRead(bufferReading.Buffer, 0, bufferReading.BufferSize, EndReadCallBack, bufferReading);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// This callback is called when the BeginRead operation is completed.
        /// We can arrive here whether the connection is valid or not
        /// </summary>
        private void EndReadCallBack(IAsyncResult result)
        {
            try
            {
                _logger.Debug("Enter in EndReadCallBack method");

                var readBytes = _pipeClient.EndRead(result);
                if (readBytes > 0)
                {
                    BufferReading reading = (BufferReading)result.AsyncState;
                    IFormatter f = new BinaryFormatter();
                    object messageReceived = f.Deserialize(new MemoryStream(reading.Buffer));

                    OnMessageReceived(messageReceived);

                    // Begin a new reading operation
                    BeginRead(new BufferReading());
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }


        public  void SendMessage(object message)
        {
            try
            {
                _logger.Debug("Enter in SendMessage method of NPClient ");
                _logger.Trace("Recived parameter  message = " + message);

                var taskCompletionSource = new TaskCompletionSource<TaskResult>();

                if (_pipeClient.IsConnected)
                {

                    MemoryStream ms = new MemoryStream();
                    BinaryFormatter.Serialize(ms, message);
                    byte[] buffer = ms.ToArray();
                    _pipeClient.BeginWrite(buffer, 0, buffer.Length, asyncResult =>
                    {
                        try
                        {
                            taskCompletionSource.SetResult(EndWriteCallBack(asyncResult));
                        }
                        catch (Exception ex)
                        {
                            taskCompletionSource.SetException(ex);
                        }

                    }, null);
                }
                else
                {
                    _logger.Error("Cannot send message, pipe is not connected");
                    throw new IOException("pipe is not connected");
                }

               // return taskCompletionSource.Task;
            }
            catch (Exception e)

            {
                _logger.Error(e);
                throw;
            }
        }

        /// <summary>
        /// This callback is called when the BeginWrite operation is completed.
        /// It can be called whether the connection is valid or not.
        /// </summary>
        /// <param name="asyncResult"></param>
        public TaskResult EndWriteCallBack(IAsyncResult asyncResult)
        {
            try
            {
                _logger.Debug("Enter in EndSendMessageCallBack");
                _pipeClient.EndWrite(asyncResult);
                _pipeClient.Flush();
                return new TaskResult { IsSuccess = true };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        #endregion
    }
}
