﻿using System;
using System.Diagnostics;
using GZipCompressionTool.Core;
using GZipCompressionTool.Core.Models;
using System.IO;

namespace GZipCompressionTool
{
    class Program
    {
        static int Main(string[] args)
        {
            // composition root
            var applicationSettings = new ApplicationSettingsProvider().GetApplicationSettings(args);
            var gZipCompressor = new GZipCompressor();
            var synchronizationContext = new CompressionSynchronizationContext(applicationSettings.ProcessorsCount);
            var executionSafeContext = new ExecutionSafeContext(synchronizationContext);
            var compressionProvider = new CompressionProvider(gZipCompressor, synchronizationContext, executionSafeContext);
            var threadPoolDispatcher = new ThreadPoolDispatcher();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

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
                    FileMode.Create,
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

                    // wait for execution
                    synchronizationContext.WaitCompletion();
                    stopWatch.Stop();
                    Console.WriteLine(stopWatch.Elapsed);
                }
            }

            return 1;
        }
    }
}
