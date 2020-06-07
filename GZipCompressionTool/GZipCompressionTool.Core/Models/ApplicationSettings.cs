﻿using System;
using System.IO.Compression;

namespace GZipCompressionTool.Core.Models
{
    public class ApplicationSettings
    {
        public CompressionMode CompressionMode { get; set; }

        public string InputFilePath { get; set; }

        public string OutputFilePath { get; set; }

        public int ChunkSize { get; } = Constants.ChunkSize;

        public int ProcessorsCount { get; } = Environment.ProcessorCount;
    }
}