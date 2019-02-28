using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace XNode {
    [System.Serializable]
    public class NodeGraphData{
        public NodeGraph graph;

        public List<NodeData> nodes = new List<NodeData>();
        public List<ConnectionData> connections = new List<ConnectionData>();

        public NodeGraphData(NodeGraph nodeGraph){
            graph = nodeGraph;
        }

        public bool RecordConnection(NodePortData portData1, NodePortData portData2)
		{
			if (!portData1.connections.Contains(portData2))
				portData1.connections.Add(portData2);

			if (!portData2.connections.Contains(portData1))
				portData2.connections.Add(portData1);


			if (!connections.Exists((ConnectionData conData) => conData.hasPort(portData1) && conData.hasPort(portData2)))
			{
				ConnectionData conData = new ConnectionData(portData1, portData2);
				connections.Add(conData);
				return true;
			}
			return false;
		}


    }

    [System.Serializable]
    public class NodeData {
        public Node node;
		public List<NodePortData> ports = new List<NodePortData>();

        public NodeData(Node n){
            node = n;
        }
    }

    [System.Serializable]
    public class NodePortData{
        public NodePort port;
        public NodeData node;
		public List<NodePortData> connections = new List<NodePortData>();

		public int portID;


		public NodePortData(NodeData nodeData, NodePort nodePort)
		{
			port = nodePort;
			portID = nodePort.GetHashCode();
			node = nodeData;
		}

    }


    [System.Serializable]
    public class ConnectionData{

 		private NodePortData port1;
		private NodePortData port2;


        public int port1ID;
        public int port2ID;

		public ConnectionData(NodePortData Port1, NodePortData Port2)
		{
			port1 = Port1;
			port2 = Port2;
            port1ID = Port1.portID;
            port2ID = Port2.portID;
		}

		public bool hasPort (NodePortData port)
		{
			return port1ID == port.portID || port2ID == port.portID;
		}
    }
}