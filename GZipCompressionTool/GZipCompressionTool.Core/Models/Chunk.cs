using System;
using System.Linq;

namespace GZipCompressionTool.Core.Models
{
    public class Chunk
    {
        public long Id { get; set; }

        public byte[] Payload { get; set; }

        public byte[] GetPayloadWithHeader()
        {
            var chunkSize = Payload.Length;
            return BitConverter.GetBytes(chunkSize).Concat(Payload).ToArray();            
        }
    }
}
