using Opc.Ua;
using Opc.Ua.Configuration;
using OPCFoundation.ServerLib.Init;
using OPCFoundation.ServerLib.Server;
using System;
using System.Threading;
using TasksLib.Tasks;


namespace OPCFoundation.ServerLib.Jobs
{
    public static class ServerJob
    {
        public static ApplicationInstance application;
        public static string JobName = "OPC UA Generic Server";
        public static CancellationTokenSource tokenSrc;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>        
        public static void Init(string configFile)
        {            
            tokenSrc = new CancellationTokenSource();
            application = new ApplicationInstance();
            application.ApplicationType = ApplicationType.Server;
            application.ConfigSectionName = JobName;
            application.CustomConfigFile = JobInit.GetConfigFilePath(configFile);

            try
            {
                application.Start(new UaServer(false)); // if true, loads default nodes from code with simulated data               
                Utils.Trace("End points: ");
                foreach (var ep in application.ApplicationConfiguration.ServerConfiguration.BaseAddresses)
                {
                    Utils.Trace("   " + ep);
                }
                ServerTask.Launch(application, 10000, "SERVER", tokenSrc);
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

        public static void Stop()
        {
            try
            {
                application.Stop();
            }
            catch (Exception e)
            {
                Utils.Trace("Error: " + e.ToString());
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
