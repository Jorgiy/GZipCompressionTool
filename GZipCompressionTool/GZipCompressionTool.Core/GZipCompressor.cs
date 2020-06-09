using GZipCompressionTool.Core.Interfaces;
using System;
using System.IO;
using System.IO.Compression;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Core
{
    public class GZipCompressor : IGZipCompressor
    {
        public byte[] Compress(byte[] input)
        {
            using (var output = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(output, CompressionMode.Compress))
                {
                    gzipStream.Write(input, 0, input.Length);
                }
                return output.ToArray();
            }               
        }

        public byte[] Decompress(byte[] input)
        {
            var decompressionResult = new byte[DecompressionBufferSize];

            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(input))
                {
                    using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        int countRead;
                        while ((countRead = gzipStream.Read(decompressionResult, 0, decompressionResult.Length)) != 0)
                        {
                            Array.Resize(ref decompressionResult, countRead);
                            outputStream.Write(decompressionResult, 0, countRead);
                        }
                    }
                }

                return outputStream.ToArray();
            }
        }
    }
}
