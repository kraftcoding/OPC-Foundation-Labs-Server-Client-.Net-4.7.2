namespace OPCFoundation.ServerLib.Constants
{
    public static class InitConstants
    {
        #region Config files paths

        // This must be changed to the correct path where the final config files are located
        public const string PATH_CONFIG_FILES = @"C:\Users\IElguezabalEXT\source\repos\OPC UA\UA-.NET_Tests\OPCFoundation-Client-Server-NET-Framework-4.5.1\OPCFoundation.ServerLib\ConfigFiles\";

        public const string SERVER_JOB_FILE = "Hangfire.ServerJob.Config.xml";
        public const string SUBSCRIBE_NODES_CLIENT_JOB_FILE = "Hangfire.SuscribeNodesClientJob.Config.xml";
        public const string NODE_WRITE_CLIENT_JOB_FILE = "Hangfire.NodeWriteClientJob.Config.xml";
        public const string NODE_READ_CLIENT_JOB_FILE = "Hangfire.NodeReadClientJob.Config.xml";

        public const string SERVER_JOB_ID = "3a9dd873-6dd3-4bf2-8dd5-1276714434c0";
        public const string SUBSCRIBE_NODES_CLIENT_JOB_ID = "3a9dd873-6dd3-4bf2-8dd5-1276714434c1";
        public const string NODE_WRITE_CLIENT_JOB_ID = "3a9dd873-6dd3-4bf2-8dd5-1276714434c2";
        public const string NODE_READ_CLIENT_JOB_ID = "3a9dd873-6dd3-4bf2-8dd5-1276714434c3";        

        #endregion

        #region Cron configurations

        public const string CRON_MINUTELY = "*/1 * * * *";

        #endregion


    }
}
