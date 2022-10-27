using System;

namespace st.rulesystem.sdk.PipeComunication.Utilities
{

    [Serializable]
    public class PipeMessage
    {
        public string topic { get; set; }
        public string Message { get; set; }

        //constructor
        public PipeMessage()
        {

        }
        public PipeMessage(string topic, string message)
        {
            this.topic = topic;
            this.Message = message;
        }
        public override string ToString()
        {
            return string.Format("[topic: {0} , Message: {1}]", topic, Message);
        }
    }


}
