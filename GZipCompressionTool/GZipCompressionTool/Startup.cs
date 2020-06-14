using System;
using System.Diagnostics;
using System.IO;
using GZipCompressionTool.Core;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;

namespace GZipCompressionTool
{
    public class Startup : IStartup
    {
        private readonly ApplicationSettings _applicationSettings;

        private readonly ICompressionProvider _compressionProvider;

        private readonly ICompressionSynchronizationContext _compressionSynchronizationContext;

        private readonly IThreadPoolDispatcher _threadPoolDispatcher = new ThreadPoolDispatcher();

        public Startup(
            ApplicationSettings applicationSettings,
            ICompressionProvider compressionProvider,
            ICompressionSynchronizationContext compressionSynchronizationContext)
        {
            _compressionProvider = compressionProvider;
            _compressionSynchronizationContext = compressionSynchronizationContext;
            _applicationSettings = applicationSettings;
        }

        public int Run(string[] args)
        {
            var executionTimer = new Stopwatch();
            executionTimer.Start();
            
            // application start
            using (var inputFileStream = new FileStream(
                _applicationSettings.InputFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                _applicationSettings.ChunkSize,
                FileOptions.Asynchronous))
            {
                using (var outputFileStream = new FileStream(
                    _applicationSettings.OutputFilePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.Write,
                    _applicationSettings.ChunkSize,
                    FileOptions.Asynchronous))
                {
                    var threadsEnumerable = _threadPoolDispatcher.GetThreads(
                        _applicationSettings.ProcessorsCount,
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
                    while (!_compressionSynchronizationContext.WaitCompletion(100))
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write($"Progress: {(inputFileStream.Position * 100 / inputFileStream.Length)} %");
                    }

                    Console.WriteLine();

                    Console.WriteLine($"Completed for {executionTimer.Elapsed}");
                    
                    executionTimer.Stop();

                    if (_compressionSynchronizationContext.ExceptionsOccured)
                    {
                        var consoleColor = Console.ForegroundColor;

                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine($"{_applicationSettings.CompressionMode} failed with errors: ");

                        foreach (var exception in _compressionSynchronizationContext.GetExceptions)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        Console.ForegroundColor = consoleColor;

                        throw new CompressFailedException();
                    }
                }
            }

            return _compressionSynchronizationContext.ExceptionsOccured ? 1 : 0;
        }
    }
}
