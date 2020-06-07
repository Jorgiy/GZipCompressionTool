namespace GZipCompressionTool.Core.Interfaces
{
    public interface IGZipCompressor
    {
        byte[] Compress(byte[] input);

        byte[] Decompress(byte[] input);
    }
}
