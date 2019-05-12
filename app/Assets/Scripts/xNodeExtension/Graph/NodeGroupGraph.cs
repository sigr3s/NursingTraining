using System;
using System.Collections.Generic;
using System.IO;
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
        public static NodeGroupGraph GroupNodes(List<Node> nodesToGroup, NodeGraph g){
            if(nodesToGroup.Count < 2){
                Debug.LogWarning("Should be 2 or more nodes to make a group");
                return null;
            }

            foreach(var node in nodesToGroup){
                if(node == null){
                    Debug.LogError("Something went wrong!");
                    return null;
                }
                node.name = node.name + "_" + node.GetHashCode();
            }

            NodeGraph gcopy = g.Copy();            
            NodeGroupGraph nodeGroupGraph = new NodeGroupGraph(Guid.NewGuid().ToString());
            nodeGroupGraph.position = nodesToGroup[0].position;
            
            nodeGroupGraph.nodes = gcopy.nodes;

            for(int i = nodeGroupGraph.nodes.Count - 1; i >= 0; i--){
                var n = nodeGroupGraph.nodes[i];
                if(nodesToGroup.Find( no => no.name == n.name ) != null){
                    Debug.LogWarning("OK¿?");
                }
                else
                {
                    nodeGroupGraph.RemoveNode(n);
                }
            }

            foreach(Node n in nodesToGroup){
                g.nodes.Remove(n);
            }           


            foreach(var n in nodeGroupGraph.nodes){
                foreach(var p in n.Ports){
                    if(!p.IsConnected){
                        nodeGroupGraph.ports.Add(p);
                    }
                }
            }

            
            if(g is NTGraph){
                ( (NTGraph) g).AddGroupedNodes( (NodeGroupGraph) nodeGroupGraph.Copy());
            }
            
            nodeGroupGraph.Export();

            return nodeGroupGraph;
        }
    }
}