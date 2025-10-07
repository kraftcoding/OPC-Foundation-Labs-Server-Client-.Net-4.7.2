using OPCFoundation.ServerLib.Constants;
using System.IO;

namespace OPCFoundation.ServerLib.Init
{
    public static class JobInit
    {
        internal static string GetConfigFilePath(string configFile, string filesPath)
        {
            if (string.IsNullOrEmpty(configFile) && string.IsNullOrEmpty(filesPath))
            {
                return null;
            }
            else
            {
                return Path.GetFullPath(filesPath + configFile);
            }
        }
    }
}
