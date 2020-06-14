using System;
using System.IO;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;

namespace GZipCompressionTool.Core
{
    public class StartupErrorHandler : IStartup
    {
        private readonly IStartup _startup;

        private readonly ApplicationSettings _applicationSettings;

        public StartupErrorHandler(IStartup startup, ApplicationSettings applicationSettings)
        {
            _startup = startup;
            _applicationSettings = applicationSettings;
        }

        public int Run(string[] args)
        {
            try
            {
                return _startup.Run(args);
            }
            catch (Exception exception)
            {
                var fileExistedBefore = false;

                if (exception is IOException ioException)
                {
                    fileExistedBefore = ioException.HResult == -2147024816 || ioException.HResult == -2147024713;
                }

                if (!fileExistedBefore && File.Exists(_applicationSettings.OutputFileFullName))
                {
                    File.Delete(_applicationSettings.OutputFileFullName);
                }

                if (exception.GetType() != typeof(CompressFailedException))
                {
                    var consoleColor = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.Error.WriteLine(exception.Message);

                    Console.ForegroundColor = consoleColor;
                }

                return 1;
            }
        }
    }
}
