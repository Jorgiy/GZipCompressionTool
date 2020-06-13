using System;
using System.IO.Compression;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Core
{
    public class ApplicationSettingsProvider : IApplicationSettingsProvider
    {
        public ApplicationSettings GetApplicationSettings(string[] args)
        {
            return false ? new ApplicationSettings
            {
                InputFilePath = @"C:\gzip\A.exe",
                OutputFilePath = @"C:\gzip\A.gz",
                ChunkSize = ChunkSize,
                CompressionMode = CompressionMode.Compress,
                ProcessorsCount =  false ? 1 : Environment.ProcessorCount
            } : new ApplicationSettings
            {
                InputFilePath = @"C:\gzip\A.gz",
                OutputFilePath = @"C:\gzip\A2.exe",
                ChunkSize = ChunkSize,
                CompressionMode = CompressionMode.Decompress,
                ProcessorsCount = false ? 1 : Environment.ProcessorCount
            };
        }
    }
}
