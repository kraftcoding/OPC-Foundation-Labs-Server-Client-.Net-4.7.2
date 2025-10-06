using OPCFoundation.ServerLibLib.Init;
using Opc.Ua;
using Opc.Ua.Configuration;
using OPCFoundation.ServerLib.Server;
using System;
using TasksLib.Tasks;


namespace OPCFoundation.ServerLibLib.Jobs
{
    public static class ServerJob
    {   
        /// <summary>
        /// The main entry point for the application.
        /// </summary>        
        public static void Init(string configFile)
        {
            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationType = ApplicationType.Server;
            application.ConfigSectionName = "Generic Server";
            application.CustomConfigFile = JobInit.GetConfigFilePath(configFile);

            try
            {
                application.Start(new UaServer(false)); // if true, loads default nodes from code with simulated data               
                Utils.Trace("End points: ");
                foreach (var ep in application.ApplicationConfiguration.ServerConfiguration.BaseAddresses)
                {
                    Utils.Trace("   " + ep);
                }
                ServerTask.Launch(application, 10000, "SERVER");
            }
            catch (Exception e)
            {
                Utils.Trace("Error: " + e.ToString());
            }
            finally
            {
                application.Stop();
            }
        }    
    }

    /// <summary>
    /// The <b>AlarmConditionServer</b> namespace contains classes which implement a UA Alarm Condition Server.
    /// </summary>
    /// <exclude/>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class NamespaceDoc
    {
    }
}
