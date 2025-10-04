using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OPCFoundation.ServerLib.Model
{
    /// <summary>
    /// Stores the configuration the data access node manager.
    /// </summary>
    
    [DataContract(Namespace = Namespaces.UaServer)]
    public class ServerNode
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public ServerNode()
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
        /// The name of the folder under which the server nodes will be organized.
        /// </summary>
        [DataMember(Order = 1)]
        public string FolderName { get; set; }

        /// <summary>
        /// The URL of the COM server which has the form: opc.com://hostname/progid/clsid
        /// </summary>
        [DataMember(Order = 2)]
        public string Bool { get; set; }

        /// <summary>
        /// The name for the server that will used for the root in the UA server address space.
        /// </summary>
        [DataMember(Order = 3)]
        public string String { get; set; }

        /// <summary>
        /// The name for the server that will used for the root in the UA server address space.
        /// </summary>
        [DataMember(Order = 4)]
        public string Byte { get; set; }

        /// <summary>
        /// The name for the server that will used for the root in the UA server address space.
        /// </summary>
        [DataMember(Order = 5)]
        public string ByteString { get; set; }

        /// <summary>
        /// The name for the server that will used for the root in the UA server address space.
        /// </summary>
        [DataMember(Order = 6)]
        public string DateTime { get; set; }

        /// <summary>
        /// The name for the server that will used for the root in the UA server address space.
        /// </summary>
        [DataMember(Order = 7)]
        public string Double { get; set; }

        #endregion

        /// <summary>
        /// A collection of COM WrapperConfiguration objects.
        /// </summary>
        [CollectionDataContract(Name = "ListOfServerNode", Namespace = Namespaces.UaServer, ItemName = "ServerNode")]
        public partial class ServerNodesCollection : List<ServerNode>
        {
            /// <summary>
            /// Initializes an empty collection.
            /// </summary>
            public ServerNodesCollection() { }

            /// <summary>
            /// Initializes the collection with the specified capacity.
            /// </summary>
            public ServerNodesCollection(int capacity) : base(capacity) { }

            /// <summary>
            /// Initializes the collection from another collection.
            /// </summary>
            /// <param name="collection">A collection of <see cref="ServerNode"/> used to pre-populate the collection.</param>
            public ServerNodesCollection(IEnumerable<ServerNode> collection) : base(collection) { }
        }
    }
}
