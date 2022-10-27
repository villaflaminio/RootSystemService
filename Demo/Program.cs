using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using st.rulesystem.sdk.PipeComunication.Client;
using st.rulesystem.sdk.PipeComunication.Interfaces;
using st.rulesystem.sdk.PipeComunication.Server;
using st.rulesystem.sdk.PipeComunication.Utilities;

namespace st.rulesystem.demo
{
    internal class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("log4net.config"));
                XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                _logger.Debug("No default logging cofiguration loaded");
            }
            
            _logger.Debug("Server c# avviato");

            IPipeClient _client = new NPClient("elis_pipe");

            _client.Start();

            object pipe = new PipeMessage("request", "client message");

            _client.MessageReceivedEvent += (sender, argss) =>
            {
                _logger.Info(" _client Message received from server " + argss.Message);
            };

            _client.SendMessage(pipe);
            _logger.Info("Message " + pipe + " sent to server");

            Console.ReadLine();
        }
    }
}
