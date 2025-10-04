using System.Runtime.Serialization;
using static OPCFoundation.ServerLib.Model.ServerNode;

namespace OPCFoundation.ServerLib.Managers
{
    /// <summary>
    /// Stores the configuration the Alarm Condition server.
    /// </summary>
    [DataContract(Namespace = OPCFoundation.ServerLib.Model.Namespaces.UaServer)]
    public class ServerConfigManager
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ServerConfigManager()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
            
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The list of COM servers wrapped by the UA server. 
        /// </summary>
        [DataMember(Order = 1)]
        public ServerNodesCollection ServerNodes
        {
            get { return m_ServerNodes; }
            set { m_ServerNodes = value; }
        }

        /// <summary>
        /// Load default configuration settings.
        /// </summary>
        [DataMember(Order = 2)]
        public bool LoadDefaults { get; set; }

        #endregion

        /// <summary>
        /// The name of the folder under which the server nodes will be organized.
        /// </summary>
        #region Private Members
        private ServerNodesCollection m_ServerNodes;
        #endregion


    }
}
