using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st.rulesystem.sdk.PipeComunication.eventClass
{
    public class ClientMessageReceivedEventArgs : EventArgs
    {
        public object Message { get; set; }
    }
}
