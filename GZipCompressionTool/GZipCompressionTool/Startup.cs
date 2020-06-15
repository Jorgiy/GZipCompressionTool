using System;
using System.Diagnostics;
using System.IO;
using GZipCompressionTool.Core;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using log4net;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool
{
    public class Startup : IStartup
    {
        private readonly ApplicationSettings _applicationSettings;

        private readonly ICompressionProvider _compressionProvider;

        private readonly ICompressionSynchronizationContext _compressionSynchronizationContext;

        private readonly IThreadPoolDispatcher _threadPoolDispatcher = new ThreadPoolDispatcher();

        private readonly ILog _logger;

        public Startup(
            ApplicationSettings applicationSettings,
            ICompressionProvider compressionProvider,
            ICompressionSynchronizationContext compressionSynchronizationContext,
            ILog logger)
        {
            _compressionProvider = compressionProvider;
            _compressionSynchronizationContext = compressionSynchronizationContext;
            _applicationSettings = applicationSettings;
            _logger = logger;
        }

        public int Run()
        {
            var executionTimer = new Stopwatch();
            executionTimer.Start();
            
            // application start
            using (var inputFileStream = new FileStream(
                _applicationSettings.InputFileFullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                _applicationSettings.ChunkSize,
                FileOptions.Asynchronous))
            {
                using (var outputFileStream = new FileStream(
                    _applicationSettings.OutputFileFullName,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.Write,
                    _applicationSettings.ChunkSize,
                    FileOptions.Asynchronous))
                {
                    var chunksInFileCount = inputFileStream.Length / ChunkSize == 0 ? 1 : inputFileStream.Length / ChunkSize;
                    var threadsCount = chunksInFileCount < _applicationSettings.ProcessorsCount
                        ? chunksInFileCount
                        : _applicationSettings.ProcessorsCount;

                    if (threadsCount > int.MaxValue)
                    {
                        threadsCount = int.MaxValue;
                    }

                    var threadsEnumerable = _threadPoolDispatcher.GetThreads(
                        (int)threadsCount,
                        () => {
                            _compressionProvider.Execute(
                                inputFileStream, outputFileStream,
                                new CompressionOptions
                                {
                                    CompressionMode = _applicationSettings.CompressionMode,
                                    ReadBufferSize = _applicationSettings.ChunkSize
                                });
                        });

                    foreach (var thread in threadsEnumerable)
                    {
                        thread.Start();
                    }
                    
                    // wait for execution
                    if (_applicationSettings.EnableProgressBar.HasValue &&
                        !_applicationSettings.EnableProgressBar.Value)
                    {
                        while (
                            !_compressionSynchronizationContext.WaitCompletion(-1))
                        {
                        }

                    }
                    else
                    {
                        while (!_compressionSynchronizationContext.WaitCompletion(100))
                        {
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write($"Progress: {(inputFileStream.Position * 100 / inputFileStream.Length)} % ");
                        }

                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write("Progress: 100 % ");
                    }

                    executionTimer.Stop();

                    _logger.Info($"Completed for {executionTimer.Elapsed}");

                    if (_compressionSynchronizationContext.ExceptionsOccured)
                    {
                        _logger.Error($"{_applicationSettings.CompressionMode} failed with errors)");

                        foreach (var exception in _compressionSynchronizationContext.GetExceptions)
                        {
                            Console.WriteLine();
                            _logger.Error(exception.Message);
                        }

                        throw new CompressFailedException();
                    }
                }
            }

            return _compressionSynchronizationContext.ExceptionsOccured ? 1 : 0;
        }
    }
}
