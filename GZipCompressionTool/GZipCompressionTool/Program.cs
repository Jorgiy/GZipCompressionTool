using System;
using System.IO;
using System.Reflection;
using GZipCompressionTool.Core;
using log4net;
using log4net.Config;

namespace GZipCompressionTool
{
    public class Program
    {
        public static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static int Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // composition root
            var applicationSettingsProvider = new ApplicationSettingsProvider();
            
            if (!applicationSettingsProvider.TryGetApplicationSettings(args, out var applicationSettings))
            {
                var consoleColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var exception in applicationSettingsProvider.Errors)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.ForegroundColor = consoleColor;

                return 1;
            }

            var gZipCompressor = new GZipCompressor();
            var synchronizationContext = new CompressionSynchronizationContext(applicationSettings.ProcessorsCount);
            var executionSafeContext = new ExecutionSafeContext(synchronizationContext);
            var compressionProvider = new CompressionProvider(gZipCompressor, synchronizationContext, executionSafeContext);
            var startup = new Startup(applicationSettings, compressionProvider, synchronizationContext, Logger);
            var startupErrorHandler = new StartupErrorHandler(startup, applicationSettings, Logger);

            // run
            return startupErrorHandler.Run();
        }
    }
}
