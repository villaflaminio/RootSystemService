using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using log4net.Config;

namespace st.rulesystemservice
{
    internal static class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            Debugger.Launch();

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("log4net.config"));
                XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
            }
            catch (Exception e)
            {
                _logger.Error(e);
                _logger.Debug("No default logging cofiguration loaded");
            }

            
            ServicesToRun = new ServiceBase[]
            {

                new RuleSystemService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
