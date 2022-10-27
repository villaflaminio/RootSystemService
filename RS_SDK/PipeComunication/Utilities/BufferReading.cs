using System.Text;

namespace st.rulesystem.sdk.PipeComunication.Utilities
{

    public class BufferReading
    {
        public readonly byte[] Buffer;
        public readonly StringBuilder StringBuilder;
        public int BufferSize { get; } = 2048;

        public BufferReading()
        {
            Buffer = new byte[BufferSize];
            StringBuilder = new StringBuilder();
        }
    }
}
