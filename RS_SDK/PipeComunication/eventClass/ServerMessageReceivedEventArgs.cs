using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st.rulesystem.sdk.PipeComunication.eventClass
{
    public class ServerMessageReceivedEventArgs : EventArgs
    {
        public string ClientId { get; set; }
        public object Message { get; set; }
    }
}
