using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using GZipCompressionTool.Core.Interfaces;
using GZipCompressionTool.Core.Models;
using static GZipCompressionTool.Core.Models.Constants;

namespace GZipCompressionTool.Core
{
    public class ApplicationSettingsProvider : IApplicationSettingsProvider
    {
        private List<UserException> _exceptions;

        public IEnumerable<UserException> Errors => _exceptions;

        public bool TryGetApplicationSettings(string[] args, out ApplicationSettings applicationSettings)
        {
            _exceptions = new List<UserException>();

            if (args.Length < 3)
            {
                _exceptions.Add(new UserException(InsufficientArgumentsErrorMessage));
                applicationSettings = null;
                return false;
            }

            var compressModeString = args[0];
            if (!Enum.TryParse(compressModeString, true, out CompressionMode compressMode))
            {
                _exceptions.Add(new UserException($"{compressModeString}{CompressModeParsingErrorMessage}"));
            }

            var inputFileName = args[1];
            if (!File.Exists(inputFileName))
            {
                _exceptions.Add(new UserException(InPutFileErrorMessage));
            }

            var outputFileName = args[2];
            string outputPath;
            try
            {
                outputPath = Path.GetDirectoryName(outputFileName);
            }
            catch (ArgumentException)
            {
                throw new UserException(InappropriateOutputFilePathError);
            }
            catch (PathTooLongException)
            {
                throw new UserException(OutputPathTooLong);
            }

            if (outputPath != string.Empty && !Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                throw new UserException(OutputDirectoryDoesNotExist);
            }
            
            if (_exceptions.Any())
            {
                applicationSettings = null;
                return false;
            }

            bool? enableProgressBar = null;

            if (args.Length > 3)
            {
                if (Enum.TryParse(args[3], true, out ProgressBar progressBarEnabled))
                {
                    enableProgressBar = progressBarEnabled == ProgressBar.Enabled;
                }
            }

            applicationSettings = new ApplicationSettings
            {
                ChunkSize = ChunkSize,
                CompressionMode = compressMode,
                InputFileFullName = inputFileName,
                OutputFileFullName = outputFileName,
                ProcessorsCount = Environment.ProcessorCount,
                EnableProgressBar = enableProgressBar
            };

            return true;
        }
    }
}
