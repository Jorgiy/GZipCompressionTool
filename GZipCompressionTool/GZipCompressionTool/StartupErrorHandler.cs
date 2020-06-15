using System;
using System.IO;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using log4net;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool
{
    public class StartupErrorHandler : IStartup
    {
        private readonly IStartup _startup;

        private readonly ApplicationSettings _applicationSettings;

        private readonly ILog _logger;

        public StartupErrorHandler(IStartup startup, ApplicationSettings applicationSettings, ILog logger)
        {
            _startup = startup;
            _applicationSettings = applicationSettings;
            _logger = logger;
        }

        public int Run()
        {
            try
            {
                return _startup.Run();
            }
            catch (Exception exception)
            {
                var fileExistedBefore = false;

                if (exception is IOException ioException)
                {
                    fileExistedBefore = ioException.HResult == HrFileExists || ioException.HResult == HrFileExistsSecondCode;
                }

                if (!fileExistedBefore && File.Exists(_applicationSettings.OutputFileFullName))
                {
                    File.Delete(_applicationSettings.OutputFileFullName);
                }

                if (exception.GetType() != typeof(CompressFailedException))
                {
                    _logger.Error(exception.Message);
                }

                return 1;
            }
        }
    }
}
