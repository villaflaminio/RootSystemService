using st.rulesystem.sdk.PipeComunication.eventClass;
using System;

namespace st.rulesystem.sdk.PipeComunication.Interfaces
{
    public interface IPipeClient : ICommunication
    {

        /// <summary>
        /// This event is fired when a message is received 
        /// </summary>
        event EventHandler<ClientMessageReceivedEventArgs> MessageReceivedEvent;
        
        /// <summary>
        /// This method sends the given message asynchronously over the communication channel
        /// </summary>
        /// <param name="message"></param>
        /// <returns>A task of TaskResult</returns>
        void SendMessage(object message);
    }
   
}
