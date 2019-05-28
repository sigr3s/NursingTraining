using System;
using System.Collections.Generic;
using System.IO;
using NT.Nodes.Messages;
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

    public class NodeGroupGraph : NTGraph {
        public static string exportPath = Application.dataPath + "/Saves/NodeGroups/";
        public static DataFormat dataFormat = DataFormat.JSON;

        public Vector2 position = Vector2.zero;
        public string assetID = "";
        public List<NodePort> ports = new List<NodePort>();
        public static Dictionary<string, NodeGroupGraph> loadedGraphs = new Dictionary<string, NodeGroupGraph>();

        public NodeGroupGraph(string id){
            this.assetID = id;
        }

        public static List<NodeGroupGraph> GetAll(){
            List<NodeGroupGraph> groupedNodes = new List<NodeGroupGraph>();
            DirectoryInfo customNodesDir = new DirectoryInfo(NodeGroupGraph.exportPath);

            if(!customNodesDir.Exists) return null;

            FileInfo[] files = customNodesDir.GetFiles("*.nt");

            foreach(var file in files){
                string assID = file.Name.Replace(".nt", "");

                if(loadedGraphs.ContainsKey(assID)){
                    groupedNodes.Add(loadedGraphs[assID]);
                }
                else
                {
                    byte[] nodeGroupData = File.ReadAllBytes(file.FullName);
                    NodeGroupGraph po = SerializationUtility.DeserializeValue<NodeGroupGraph>(nodeGroupData, dataFormat);
                    groupedNodes.Add(po);
                    loadedGraphs.Add(po.assetID, po);
                }
            }
            return groupedNodes;
        }

        public static void Remove(string assetID){
            string path = NodeGroupGraph.exportPath + assetID + ".nt";
            if(File.Exists(path)){
                File.Delete(path);
            }

            if(loadedGraphs.ContainsKey(assetID)){
                loadedGraphs.Remove(assetID);
            }

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

                if((node is IVariableNode) ){
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

                for(int i = g.nodes.Count -1; i >= 0; i--){
                    Node n = g.nodes[i];
                    var nodeInNG = ngc.nodes.Find( no => no.name == n.name );

                    if(nodeInNG == null){

                        /*foreach(var port in n.Ports){
                            for (int c = port.ConnectionCount - 1 ; c >= 0; c--) {
                                NodePort other = port.GetConnection(c);
                                
                                Node nodeInGroup = ngc.nodes.Find( no => no.name == other.node.name);

                                if(nodeInGroup != null){

                                    Debug.Log("Redirect it to??? __ " + n.name);

                                    port.Redirect(new List<Node>(){other.node}, new List<Node>(){nodeInGroup});
                                }
                            }
                        }*/
                    }
                    else
                    {
                        foreach(var port in n.Ports){

                            for (int c = port.ConnectionCount - 1 ; c >= 0; c--) {
                                NodePort other = port.GetConnection(c);

                                Node nodeInGroup = ngc.nodes.Find( no => no.name == other.node.name);

                                if(nodeInGroup == null){
                                    nodeInNG.GetPort(port.fieldName).Connect(other);
                                }
                            }
                        }

                        g.nodes.Remove(n);

                    }

                }
            }
            
            nodeGroupGraph.Export();

            return nodeGroupGraph;
        }

    }
}