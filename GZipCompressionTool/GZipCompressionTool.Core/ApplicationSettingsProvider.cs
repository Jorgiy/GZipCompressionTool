using System;
using System.Collections.Generic;
using System.IO.Compression;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Core
{
    public class ApplicationSettingsProvider : IApplicationSettingsProvider
    {
        private readonly List<UserException> _exceptions = new List<UserException>();

        public IEnumerable<UserException> Errors => _exceptions;

        public bool TryGetApplicationSettings(string[] args, out ApplicationSettings applicationSettings)
        {
            if (args.Length < 3)
            {
                _exceptions.Add(new UserException());
            }
            var inputFilePathString = 
            applicationSettings = 
            false ? new ApplicationSettings
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

            return true;
        }
    }
}
