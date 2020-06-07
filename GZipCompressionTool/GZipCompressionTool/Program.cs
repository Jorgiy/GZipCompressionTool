using GZipCompressionTool.Core;
using GZipCompressionTool.Core.Models;
using System.IO;

namespace GZipCompressionTool
{
    class Program
    {
        static void Main(string[] args)
        {
            // composition root
            var applicationSettings = new ApplicationSettingsProvider().GetApplicationSettings(args);
            var gZipIO = new GZipIO();
            var gZipCompressor = new GZipCompressor();
            var synchronizationContext = new CompressionSynchronizationContext(applicationSettings.ProcessorsCount);
            var compressionProvider = new CompressionProvider(gZipCompressor, synchronizationContext, gZipIO);
            var threadPoolDispatcher = new ThreadPoolDispatcher();

            // application start
            using (var inputFileStream = new FileStream(
                applicationSettings.InputFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                applicationSettings.ChunkSize,
                FileOptions.Asynchronous))
            {
                using (var outputFileStream = new FileStream(
                    applicationSettings.OutputFilePath,
                    FileMode.Open,
                    FileAccess.Write,
                    FileShare.Write,
                    applicationSettings.ChunkSize,
                    FileOptions.Asynchronous))
                {
                    var threadsEnumerable = threadPoolDispatcher.GetThreads(
                        applicationSettings.ProcessorsCount, 
                        () => { compressionProvider.Execute(
                            inputFileStream, outputFileStream, 
                            new CompressionOptions { 
                                CompressionMode = applicationSettings.CompressionMode, 
                                ReadBufferSize = applicationSettings.ChunkSize }); 
                        });

                    foreach(var thread in threadsEnumerable)
                    {
                        thread.Start();
                    }
                }
            }

            // wait for execution
            synchronizationContext.WaitCompletion();
        }
    }
}
