using DB.ModelLib.Managers;
using Opc.Ua;
using Opc.Ua.Configuration;
using OPCFoundation.ClientLib.Client;
using OPCFoundation.ClientLib.Helpers;
using OPCFoundation.ServerLib.Init;
using OPCFoundation.TaskLib.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using OPCFoundation.TaskLib.Tasks;

namespace OPCFoundation.ServerLib.Jobs
{
    public class SuscribeNodesClientJob : TaskBase
    {
        public static ApplicationInstance application;
        public static string JobName = "OPC UA Suscriber Client";
        public static CancellationTokenSource tokenSrc;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>  
        public static void Init(string configFile, string filesPath)
        {
            #region Global variables

            //Params
            bool p_useSecurity = false;
            int p_baseAddressId = 0; //default 0
            string ns = "ns=2;s="; //default ns=2;s=
            string appName = "Client Test App 2";
            string appConfig = "ClientTestAppConfig";

            //Test subscription
            Dictionary<string, string> subsDictionary = new Dictionary<string, string>();
            subsDictionary.Add("DisplayName", "Alarm");
            subsDictionary.Add("PublishingInterval", "1000");
            subsDictionary.Add("KeepAliveCount", "10");
            subsDictionary.Add("LifetimeCount", "100");
            subsDictionary.Add("MaxNotificationsPerPublish", "1000");
            subsDictionary.Add("PublishingEnabled", "true");

            #endregion

            tokenSrc = new CancellationTokenSource();
            ProcessModelContext context = new ProcessModelContext();
            UaClient Client = new UaClient(p_baseAddressId, appName, appConfig, context, JobInit.GetConfigFilePath(configFile, filesPath));

            try
            {
                // Stablish comunication with server
                Client.ConnectEndPoint(p_useSecurity);
                Utils.Trace("Connected to: " + Client.m_session.Endpoint.EndpointUrl.ToString());              
                                
                string[] nodeIds = ConfigHelper.GetConfigValues(Client, ns);
                foreach (var nodeId in nodeIds)
                    Client.CreateSubscription(nodeId, "NODE #" + nodeId + "#", subsDictionary, MonitoringMode.Reporting);
                
                ClientTask ClientTsk = new ClientTask();
                ClientTsk.Launch(Client, 60000, "CLIENT (" + appName + ")", tokenSrc);
            }
            catch (Exception exception)
            {
                Utils.Trace("Error: " + exception.ToString());
            }
            finally
            {
                Client.DisconnectEndPoint();
            }

        }

        public static void Stop()
        {
            try
            {
                tokenSrc.Cancel();
                application.Stop();
            }
            catch (Exception e)
            {
                Utils.Trace("Error: " + e.ToString());
            }
        }
    }
}
