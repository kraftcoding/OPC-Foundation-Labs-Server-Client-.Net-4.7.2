using Hangfire.OPC.JobLib.Constants;
using System.IO;

namespace Hangfire.OPC.JobLib.Init
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
