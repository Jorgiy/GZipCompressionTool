using GZipCompressionTool.Core.Models;

namespace GZipCompressionTool.Core.Interfaces
{
    public interface IApplicationSettingsProvider
    {
        ApplicationSettings GetApplicationSettings(string[] args);
    }
}
