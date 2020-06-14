using System;
using GZipCompressionTool.Core;

namespace GZipCompressionTool
{
    class Program
    {
        static int Main(string[] args)
        {
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
            var startup = new Startup(applicationSettings, compressionProvider, synchronizationContext);
            var startupErrorHandler = new StartupErrorHandler(startup, applicationSettings);

            // run
            return startupErrorHandler.Run(args);
        }
    }
}
