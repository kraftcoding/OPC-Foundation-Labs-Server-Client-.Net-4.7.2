using OPCFoundation.ServerLibLib.Constants;
using System.IO;

namespace OPCFoundation.ServerLibLib.Init
{
    public static class JobInit
    {
        internal static string GetConfigFilePath(string configFile)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                return null;
            }
            else
            {
                return Path.GetFullPath(InitConstants.PATH_CONFIG_FILES + configFile);
            }
        }
    }
}
