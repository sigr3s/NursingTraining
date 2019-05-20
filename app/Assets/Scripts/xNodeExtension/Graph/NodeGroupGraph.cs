using System;
using System.Collections.Generic;
using System.IO;
using NT.Nodes.Variables;
using OdinSerializer;
using UnityEngine;
using XNode;

namespace  NT.Graph
{
    public static class NodeGroupGraphExtensions{
        public static void Export(this NodeGroupGraph graph){
            byte[] serializedData = SerializationUtility.SerializeValue(graph, NodeGroupGraph.dataFormat);
            File.WriteAllBytes(NodeGroupGraph.exportPath + graph.assetID + ".nt", serializedData);
        }

        public static NodeGroupGraph AddTo(this NodeGroupGraph graph, NTGraph parentGraph, Vector2 position){
            var ngc = (NodeGroupGraph) graph.Copy();
            ngc.position = position;
            parentGraph.AddGroupedNodes(ngc);
            return ngc;
        }
    }
    
    public class NodeGroupGraph : NodeGraph {
        public static string exportPath = Application.dataPath + "/Saves/NodeGroups/";
        public static DataFormat dataFormat = DataFormat.JSON;

        public Vector2 position = Vector2.zero;
        public string assetID = "";
        public List<NodePort> ports = new List<NodePort>();

        public NodeGroupGraph(string id){
            this.assetID = id;
        }
        public static List<NodeGroupGraph> GetAll(){
            List<NodeGroupGraph> groupedNodes = new List<NodeGroupGraph>();
            DirectoryInfo prefabsDir = new DirectoryInfo(NodeGroupGraph.exportPath);

            if(!prefabsDir.Exists) return null;

            FileInfo[] files = prefabsDir.GetFiles("*.nt");

            foreach(var file in files){
                byte[] nodeGroupData = File.ReadAllBytes(file.FullName);
                NodeGroupGraph po = SerializationUtility.DeserializeValue<NodeGroupGraph>(nodeGroupData, dataFormat);
                groupedNodes.Add(po);
            }
            return groupedNodes;
        }
        public static NodeGroupGraph GroupNodes(List<Node> nodesToGroup, NodeGraph g, string name = "Nodes Group"){
            if(nodesToGroup.Count < 2){
                Debug.LogWarning("Should be 2 or more nodes to make a group");
                return null;
            }

            for(int i = nodesToGroup.Count - 1; i >= 0; i--){
                Node node = nodesToGroup[i];
                if(node == null){
                    Debug.LogError("Something went wrong!");
                    return null;
                }

                if((node is GetNTVariableNode) || (node is SetNTVariableNode) ){
                    nodesToGroup.Remove(node);
                    Debug.LogWarning("Removed GET/SET");
                }
                else
                {
                    node.name = node.name + "_" + node.GetHashCode();
                }
            }

            NodeGraph gcopy = g.Copy();            
            NodeGroupGraph nodeGroupGraph = new NodeGroupGraph(Guid.NewGuid().ToString());
            nodeGroupGraph.name = name;
            
            nodeGroupGraph.nodes = gcopy.nodes;

            for(int i = nodeGroupGraph.nodes.Count - 1; i >= 0; i--){
                var n = nodeGroupGraph.nodes[i];
                Node nod = nodesToGroup.Find( no => no.name == n.name );
                

                // node is not contained in the group => remove it from the grouped graph!
                if(nod == null){
                    foreach(var port in n.Ports){
		                for (int c = port.ConnectionCount - 1 ; c >= 0; c--) {
                            NodePort other = port.GetConnection(c);
                            
                            //if this node is in the grouped ones then we should remove it from group graph but keep the connection
                            Node nodeInGroup = nodesToGroup.Find( no => no.name == other.node.name);

                            if(nodeInGroup != null){
                                port.Redirect(new List<Node>(){other.node}, new List<Node>(){nodeInGroup});
                                Debug.Log("Rdirect connection?");
                            }
                        }
                    }
                    nodeGroupGraph.RemoveNode(n);
                }
            }

            foreach(var n in nodeGroupGraph.nodes){
                foreach(var p in n.Ports){
                    if(!p.IsConnected){
                        nodeGroupGraph.ports.Add(p);
                    }
                }
            }

            
            if(g is NTGraph){
                NodeGroupGraph ngc = nodeGroupGraph.AddTo((NTGraph) g, nodesToGroup[0].position);

                foreach(Node n in nodesToGroup){
                    g.nodes.Remove(n);
                }
            }
            
            nodeGroupGraph.Export();

            return nodeGroupGraph;
        }

    }
}