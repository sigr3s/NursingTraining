using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using System;

namespace XNode.InportExport {
    public class JSONImportExport : ImportExportFormat
    {
        //  FIXME: Generic export when recieves ignore field parameters
        // Add attribute like IgnoreExport or similar
        // For linked types do similar
        public override void ExportData(NodeGraphData data, string path, List<Type> referenceTypes)
        {
            JSONObject exportJSON = new JSONObject();

            JSONObject graph = new JSONObject();

            JSONArray graphNodesReferences = new JSONArray();

            referenceTypes.Add(typeof(Node));
            referenceTypes.Add(typeof(NodeGraph));
            referenceTypes.Add(typeof(NodePort));

            graph.Add(  "type"  , new JSONString(data.graph.GetType().AssemblyQualifiedName) );
            graph.Add(  "name"  , new JSONString(data.graph.name)               );
            graph.Add(  "id"    , new JSONNumber(data.graph.GetHashCode())      );
            NodeGraph g = data.graph;
            graph = (JSONObject) SimpleJSONExtension.ToJSON(g, graph, new List<string>() {"sceneVariables"}, referenceTypes);

            JSONArray connections = new JSONArray();

            for(int c = 0; c < data.connections.Count; c++){
                JSONObject connection = new JSONObject();
                connection.Add("port1ID", data.connections[c].port1ID);
                connection.Add("port2ID", data.connections[c].port2ID);
                connections.Add(connection);
            }

            JSONArray nodes = new JSONArray();

            for(int n = 0; n < data.nodes.Count; n++){
                Node node = data.nodes[n].node;
                JSONObject nodeJSON = new JSONObject();
                nodeJSON.Add("name" ,  node.name );
                nodeJSON.Add("type" , node.GetType().AssemblyQualifiedName);
                nodeJSON.Add("position", node.position);
                nodeJSON.Add("id", node.GetHashCode());

                JSONArray nodePorts = new JSONArray();

                for(int np = 0; np < data.nodes[n].ports.Count; np++){
                    NodePort port = data.nodes[n].ports[np].port;

                    JSONObject nodePortJSON = new JSONObject();
                    nodePortJSON.Add("name", port.fieldName);
                    nodePortJSON.Add("id", port.GetHashCode());
                    nodePortJSON.Add("dynamic", port.IsDynamic);
                    nodePortJSON.Add("valueType", port.ValueType.AssemblyQualifiedName);
                    nodePortJSON.Add("typeConstraint", (int) port.typeConstraint);
                    nodePortJSON.Add("connectionType", (int) port.connectionType);
                    nodePortJSON.Add("direction", (int) port.direction);

                    nodePorts.Add(nodePortJSON);
                }

                nodeJSON.Add("ports", nodePorts);

                nodeJSON = (JSONObject) SimpleJSONExtension.ToJSON(node, nodeJSON, new List<string>(), referenceTypes);

                nodes.Add(nodeJSON);
            }

            exportJSON.Add("graph", graph);
            exportJSON.Add("connections", connections);
            exportJSON.Add("nodes", nodes);

            if(File.Exists(path)){
                File.Delete(path);
            }

            File.WriteAllText(path, exportJSON.ToString());
        }


        public override NodeGraphData ImportData(string path)
        {
            NodeGraphData returnData = null;

            if(File.Exists(path)){
                string jsonString =  File.ReadAllText(path);
                JSONNode root = JSON.Parse(jsonString);

                Dictionary<int, object> references = new Dictionary<int, object>();
                Dictionary<NodePort, NodePortData> portDatas = new Dictionary<NodePort, NodePortData>();

                List<string> ignoredFields =  new List<string> {"name", "graph", "ports", "nodes"};
                JSONObject graphJObject;

                if(root.HasKey("graph")){
                    graphJObject = root["graph"].AsObject;
                    int id = graphJObject["id"];
                    string graphTypeS = graphJObject["type"];
                    Type graphType = Type.GetType(graphTypeS);

                    NodeGraph  graph = (NodeGraph) ScriptableObject.CreateInstance(graphType);
                    graph.name = graphJObject["name"];

                    references.Add(id, graph);
                    returnData = new NodeGraphData(graph);

                    Debug.Log("Basic graph OK!");
                }
                else
                {
                    Debug.LogWarning("Basic graph KO!");
                    return returnData;
                }

                if(root.HasKey("nodes")){
                    JSONArray nodesJArray = root["nodes"].AsArray;

                    foreach (JSONObject nodeJObject in nodesJArray.Values)
                    {
                        int id = nodeJObject["id"];
                        string nodeTypeS = nodeJObject["type"];
                        Type nodeType = Type.GetType(nodeTypeS);

                        Node  node = (Node) ScriptableObject.CreateInstance(nodeType);
                        node.name = nodeJObject["name"];
                        object nodeOBJ = (object) node;

                        SimpleJSONExtension.FromJSON(ref  nodeOBJ, nodeType, nodeJObject, ignoredFields, references);
                        node.graph = returnData.graph;
                        NodeData nodeData = new NodeData(node);

                        JSONArray nodePortsArray = nodeJObject["ports"].AsArray;
                        foreach (var nodePort in nodePortsArray.Values)
                        {
                            bool dynamic = nodePort["dynamic"].AsBool;
                            string portName = nodePort["name"];
                            NodePort port = null;
                            int portId = 0;

                            if(dynamic){
                                if(!node.HasPort(portName)){
                                    Type dynamicType = Type.GetType(nodePort["valueType"]);
                                    Node.TypeConstraint constraint = (Node.TypeConstraint) nodePort["typeConstraint"].AsInt;
                                    Node.ConnectionType connectionType = (Node.ConnectionType) nodePort["connectionType"].AsInt;
                                    NodePort.IO direction = (NodePort.IO) nodePort["direction"].AsInt;

                                    if(direction == NodePort.IO.Input){
                                        port = node.AddInstanceInput(dynamicType, connectionType, constraint, portName);
                                    }
                                    else
                                    {
                                        port = node.AddInstanceOutput(dynamicType, connectionType, constraint, portName);
                                    }

                                }
                                else
                                {
                                    Debug.LogWarning("Ignoring port bc is already created");
                                }
                            }

                            port = node.GetPort(nodePort["name"]);
                            portId = nodePort["id"];

                            NodePortData portData = new NodePortData(nodeData, port);

                            nodeData.ports.Add(portData);
                            portDatas.Add(port, portData);
                            references.Add(portId, port);
                        }

                        references.Add(id, node);
                        returnData.nodes.Add(nodeData);
                    }

                    Debug.Log("Basic Nodes OK!");

                }
                else
                {
                    Debug.LogWarning("Basic Nodes KO!");
                    return returnData;
                }


                if(root.HasKey("connections")){
                    JSONArray connectionsJArray = root["connections"].AsArray;

                    foreach (JSONObject connectionJObject in connectionsJArray.Values)
                    {
                        int port1ID = connectionJObject["port1ID"].AsInt;
                        int port2ID = connectionJObject["port2ID"].AsInt;

                        if(references.ContainsKey(port1ID) && references.ContainsKey(port2ID)){
                            NodePort p1 =  (NodePort) references[port1ID];
                            NodePort p2 =  (NodePort) references[port2ID];

                            p1.Connect(p2);
                        }
                        else
                        {
                            Debug.LogWarning("Error recovering one connection");
                        }

                    }
                    Debug.Log("Connections OK!");
                }
                else
                {
                    Debug.LogWarning("Connections KO!");
                    return returnData;
                }

                
                object graphObject = returnData.graph;
                SimpleJSONExtension.FromJSON(ref  graphObject, returnData.graph.GetType(), graphJObject , ignoredFields, references);
            }
            else{
                Debug.LogError("Path does not exist " + path);
            }




            return returnData;
        }
    }
}