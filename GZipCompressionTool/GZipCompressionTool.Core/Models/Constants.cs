namespace GZipCompressionTool.Core.Models
{
    public static class Constants
    {
        public const int ChunkHeaderSize = 4;

        public const int DecompressionBufferSize = 1024 * 1024;

        public const int ChunkSize = 4* 1024 * 1024;

        public const string UserInstruction = "";

        public const string IoErrorMessage = "Error occured while reading/writing: ";

        public const string OtherErrorsMessage = "Unknown error occured: ";

        public const string InvalidDataStreamMessage = "Input file is in incorrect format.";

        public const string InsufficientArguments = "Insufficient passed arguments: ";
    }
}
