using DB.ModelLib.Managers;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using OPCFoundation.ClientLib.Controllers;
using System.Collections.Generic;

namespace OPCFoundation.ClientLib.Client
{
    public class UaClient
    {
        #region Public objects
        public Session m_session;
        public ApplicationConfiguration m_configuration;
        #endregion

        #region Private objects
        private ApplicationInstance application;        
        private string m_endPoint;        
        private ConnectivityManager ConnCtrl;
        public ProcessModelContext m_context;        
        #endregion

        public UaClient(int baseAddressId, string ApplicationName, string ConfigSectionName, ProcessModelContext context, string customConfigPath)
        {
            // application config. section
            application = new ApplicationInstance();
            application.ApplicationName = ApplicationName;
            application.ConfigSectionName = ConfigSectionName;
            application.ApplicationType = ApplicationType.Client;
            application.CustomConfigFile = customConfigPath;

            // load the application configuration.
            application.LoadApplicationConfiguration(false);

            // check the application certificate.
            application.CheckApplicationInstanceCertificate(false, 0);

            // initialize connection
            m_configuration = application.ApplicationConfiguration;

            m_endPoint = m_configuration.ServerConfiguration.BaseAddresses[baseAddressId];

            // create connectivity controler
            ConnCtrl = new ConnectivityManager(m_configuration);
            
            m_context = context;
        }

        #region Public objects
        public void ConnectEndPoint(bool useSecurity)
        {
            m_session = ConnCtrl.Connect(m_endPoint, useSecurity);
            Utils.Trace("Session opened");
        }

        public void CreateSubscription(string nodeId, string name, Dictionary<string, string> subsDictionary, MonitoringMode monitoringMode)
        {
            SubscriptionsManager Subs = new SubscriptionsManager(m_context);
            Subscription m_subscription = null;

            if (subsDictionary == null)
            {
                m_subscription = Subs.CreateDefault(m_session);
            }
            else
            {
                m_subscription = Subs.CreateByDictionary(m_session, subsDictionary);
            }

            m_subscription.Create();
            Utils.Trace("Suscription created");

            // default
            MonitoredItem m_monitoredItem = Subs.GetMonitoredItem(nodeId, name, monitoringMode, null);

            // filtered (not working)
            // MonitoredItem m_monitoredItem = Subs.GetMonitoredItem(nodeId, name, monitoringMode, m_session);   

            m_subscription.AddItem(m_monitoredItem);
            Utils.Trace("Adding monitored item");

            m_subscription.ApplyChanges();            

            m_session.AddSubscription(m_subscription);
            Utils.Trace("Apply changes");

        }

        public void PrintNodesToLog()
        {
            NodeManager Nods = new NodeManager();
            Nods.PrintRootFolderToLog(m_session);
        }

        public void DisconnectEndPoint()
        {
            ConnCtrl.Disconnect();
            Utils.Trace("Session closed");
        }
        #endregion
    }
}
