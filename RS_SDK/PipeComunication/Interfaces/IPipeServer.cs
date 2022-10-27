using System;

namespace st.rulesystem.sdk.PipeComunication.Interfaces
{
    public interface IPipeServer : ICommunication
    {
        /// <summary>
        /// The server id
        /// </summary>
        string ServerId { get; }
        /// <summary>
        /// This event is fired when a message is received 
        /// </summary>
        event EventHandler<ServerMessageReceivedEventArgs> MessageReceivedEvent;

        /// <summary>
        /// This event is fired when a client connects 
        /// </summary>
        event EventHandler<ClientConnectedEventArgs> ClientConnectedEvent;

        /// <summary>
        /// This event is fired when a client disconnects 
        /// </summary>
        event EventHandler<ClientDisconnectedEventArgs> ClientDisconnectedEvent;
       
        /// <summary>
        /// This method sends the given message asynchronously over the communication channel
        /// </summary>
        /// <param name="message"></param>
        /// <returns>A task of TaskResult</returns>
        void SendMessage(string clientID, object message);
    }
    public class ServerMessageReceivedEventArgs : EventArgs
    {
        public string ClientId { get; set; }
        public object Message { get; set; }
    }

}
