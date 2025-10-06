using DB.ModelLib.Managers;
using OPCFoundation.ServerLib.Init;
using Opc.Ua;
using OPCFoundation.ClientLib.Client;
using OPCFoundation.ClientLib.Helpers;
using System;
using System.Collections.Generic;

namespace OPCFoundation.ServerLib.Jobs
{
    public static class NodeReadClientJob
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>  
        public static void Init(string configFile)
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

            ProcessModelContext context = new ProcessModelContext();
            UaClient Client = new UaClient(p_baseAddressId, appName, appConfig, context, JobInit.GetConfigFilePath(configFile));

            try
            {
                // Stablish comunication with server
                Client.ConnectEndPoint(p_useSecurity);
                Utils.Trace("Connected to: " + Client.m_session.Endpoint.EndpointUrl.ToString());

                string[] nodeIds = ConfigHelper.GetConfigValues(Client, ns);                
                Launch(Client, nodeIds);
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
    

        public static void Launch(UaClient Client, string[] nodeIds)
        {
            try
            {
                ReadNodes(Client, nodeIds);
            }
            catch (OperationCanceledException)
            {
                Utils.Trace("Task was cancelled by user");
            }
            catch (Exception ex)
            {
                Utils.Trace("Error: " + ex.ToString());
            }           

            Utils.Trace("Program completed");
        }        

        internal static void ReadNodes(UaClient Prg, string[] nodeIds)
        {
            List<NodeId> variableIds = new List<NodeId>();
            List<Type> expectedTypes = new List<Type>();

            List<object> values;
            List<ServiceResult> errors;

            foreach (var nodeId in nodeIds)
            {
                variableIds.Add(new NodeId(nodeId));
                expectedTypes.Add(null);
            }

            Prg.m_session.ReadValues(variableIds, expectedTypes, out values, out errors);

            int i = 0;
            foreach (var value in values)
            {
                if (errors[i].Code != Opc.Ua.StatusCodes.Good)
                {
                    Utils.Trace("Error: failed to read data");
                }
                else
                {
                    NodeId node = variableIds[i];
                    Utils.Trace("Read value from node {0} is {1}", node , value);
                }
                
                i++;
            }
        }
    }
}
