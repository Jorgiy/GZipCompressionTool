﻿using System;
using GZipCompressionTool.Core.Models;
using System.IO;
using System.IO.Compression;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IExecutionContext : IDisposable
    {
        Stream InputStream { get; }

        Stream OutputStream { get; }

        Chunk Chunk { get; set; }

        int ChunkSize { get; set; }

        IGZipIO GZipIo { get; set; }

        CompressionMode CompressionMode { get; set; }
    }
}
