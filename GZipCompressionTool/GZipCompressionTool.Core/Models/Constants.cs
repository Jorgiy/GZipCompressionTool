namespace GZipCompressionTool.Core.Models
{
    public static class Constants
    {
        public const int ChunkHeaderSize = 4;

        public const int DecompressionBufferSize = 1024 * 1024;

        public const int ChunkSize = 1024 * 1024;
        
        public const string IoErrorMessage = "Error occured while reading/writing: ";

        public const string OtherErrorsMessage = "Unknown error occured: ";

        public const string InvalidDataStreamMessage = "Input file is in incorrect format.";

        public const string InsufficientArgumentsErrorMessage = 
            "Insufficient passed arguments: arguments should be [compress/decompress] [input file name] [output file name] [optional progress bar:enabled/disabled]";

        public const string CompressModeParsingErrorMessage = " is not appropriate value for compression mode, please use comrpess or decompress";

        public const string InPutFileErrorMessage = "Input file does not exist";

        public const string InappropriateOutputFilePathError = "Output file name/path named incorrectly";

        public const string OutputPathTooLong = "Output file path is too long";

        public const string OutputDirectoryDoesNotExist = "Output directory does not exist";
    }
}
