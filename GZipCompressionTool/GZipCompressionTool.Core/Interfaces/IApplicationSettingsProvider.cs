using System.Collections.Generic;
using GZipCompressionTool.Core.Models;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IApplicationSettingsProvider
    {
        bool TryGetApplicationSettings(string[] args, out ApplicationSettings applicationSettings);

        IEnumerable<UserException> Errors { get; }
    }
}
