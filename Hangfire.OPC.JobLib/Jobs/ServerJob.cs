using Hangfire.OPC.Configuration.Logs;
using Hangfire.OPC.JobLib.Base;
using Hangfire.OPC.JobLib.Init;
using Opc.Ua;
using Opc.Ua.Configuration;
using OPCFoundation.ServerLib.Server;
using OPCFoundation.TaskLib.Tasks;
using System;
using System.Threading;

namespace Hangfire.OPC.JobLib.Jobs
{    
    public class ServerJob : JobBase
    {
        public static string JobName = "OPC UA Generic Server";
        public static ApplicationInstance application;        
        public static CancellationTokenSource tokenSrc;        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>        
        public static void Init(string configFile, string filesPath)
        {
            Utils.Trace("Initiating job... {0}", JobName);
            TextBuffer.WriteLine(string.Format("Initiating job... {0}", JobName));

            try
            {
                tokenSrc = new CancellationTokenSource();
                application = new ApplicationInstance();
                application.ApplicationType = ApplicationType.Server;
                application.ConfigSectionName = JobName;
                application.CustomConfigFile = JobInit.GetConfigFilePath(configFile, filesPath);            
                
                application.Start(new UaServer(false)); // if true, loads default nodes from code with simulated data
                
                Utils.Trace("End points: ");
                TextBuffer.WriteLine("End points: ");
                foreach (var ep in application.ApplicationConfiguration.ServerConfiguration.BaseAddresses)
                {
                    Utils.Trace("   " + ep);
                    TextBuffer.WriteLine(string.Format("   {0}", ep));
                }

                string taskname = "ServerTask";
                Utils.Trace("Launching task... {0}", taskname);
                TextBuffer.WriteLine(string.Format("Launching task... {0}", taskname));                
                ServerTask.Launch(application, 10000, taskname, tokenSrc);
            }
            catch (OperationCanceledException ex)
            {
                Utils.Trace("Task was cancelled by user");
                TextBuffer.WriteLine(string.Format("Task was cancelled by user"));
            }
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());
                TextBuffer.WriteLine(string.Format("Error: {0}", ex.ToString()));
                TextBuffer.WriteLine(string.Format("StacTrace: {0}", ex.StackTrace));

                Stop();
            }
            finally
            {
                Utils.Trace("Program completed");
                TextBuffer.WriteLine("Program completed");                
            }
        }

        public static void Stop()
        {
            try
            {
                Utils.Trace("Stoping JOB... {0}", JobName);
                TextBuffer.WriteLine(string.Format("Stoping... {0}", JobName));
                tokenSrc.Cancel();
                tokenSrc.Dispose();
                application.Stop();
            }
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());
                //TextBuffer.WriteLine(string.Format("Error: {0}", ex.ToString()));
                //TextBuffer.WriteLine(string.Format("StacTrace: {0}", ex.StackTrace));
            }            
        }
    }
}
