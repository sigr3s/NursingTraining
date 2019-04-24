
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NT.Nodes.Messages;
using NT.Variables;
using UnityEditor;
using UnityEngine;
using XNode;
using XNode.InportExport;

namespace  NT.Graph
{
    public class NTGraph : NodeGraph {
        [Header("Execution Flow Nodes")]
        public List<NTNode> executionNodes = new List<NTNode>();

        public virtual List<string> GetCallbacks()
        {
            return new List<string>();
        }

        public List<CallbackNode> callbackNodes = new List<CallbackNode>();



        [Header("References")]
        public SceneVariables sceneVariables;

        public Dictionary<string, List<CallbackNode>> callbackNodesDict = new Dictionary<string, List<CallbackNode>>();

        public override Node AddNode(System.Type type){
            Node node = base.AddNode(type);
            if(node is CallbackNode){
                callbackNodes.Add( (CallbackNode) node );
            }
            return node;
        }

        public virtual void GenerateCallbackDict()
        {
            callbackNodesDict = new Dictionary<string, List<CallbackNode> >();
            foreach(CallbackNode cn in callbackNodes){
                if(cn != null && !string.IsNullOrEmpty(cn.callbackKey) ){
                    List<CallbackNode> callbacksInKey = new List<CallbackNode>();

                    if(callbackNodesDict.ContainsKey(cn.callbackKey)){
                        callbacksInKey = callbackNodesDict[cn.callbackKey];
                        callbacksInKey.Add(cn);
                        callbackNodesDict[cn.callbackKey] = callbacksInKey;
                    }
                    else
                    {
                        callbacksInKey.Add(cn);
                        callbackNodesDict[cn.callbackKey] = callbacksInKey;
                    }
                }
            }
        }

        public virtual void MessageRecieved(string message)
        {
            Debug.Log("Message recieved!   " + message);
            if(!string.IsNullOrEmpty(message) && callbackNodesDict.ContainsKey(message)){
                List<CallbackNode> nodesToExecute = callbackNodesDict[message];
                foreach(CallbackNode cn in nodesToExecute){
                    CoroutineRunner.Instance.StartCoroutine(StartExecutionFlow(cn));
                }
            }
        }

        public static Node GroupNodes(List<Node> nodesToGroup)
        {
            Debug.LogWarning("Not yet implemented");
            
            //Get all inputs
            //Get all ports
            foreach(Node n in nodesToGroup){

            }

            return null;
        }

        public IEnumerator StartExecutionFlow(CallbackNode callbackNode)
        {
            NodeExecutionContext nodeExecutionContext = new NodeExecutionContext{node = callbackNode};

            while(nodeExecutionContext.node != null){

                Debug.Log("Execute node:  " + nodeExecutionContext.node.name);
                nodeExecutionContext.node.Enter();

                yield return new YieldNode(nodeExecutionContext );

                Debug.Log("Finished node:  " + nodeExecutionContext.node.name);

                yield return new WaitForSeconds(1.25f);

                Debug.Log("Finished waiting?:  " + nodeExecutionContext.node.name);

                nodeExecutionContext.node.Exit();

                nodeExecutionContext = nodeExecutionContext.node.NextNode(nodeExecutionContext);
            }

            yield return null;
        }

        Dictionary<Type, Type> dataToNtVatiable = new Dictionary<Type, Type>();

        public Type GetVariableFor(Type t){
            if(dataToNtVatiable.ContainsKey(t)) return dataToNtVatiable[t];


            foreach(Type nodeType in ReflectionUtilities.variableNodeTypes){
                if(nodeType.IsGenericTypeDefinition){
                    continue;
                }

                Type dataType = ((NTVariable) Activator.CreateInstance(nodeType)).GetDataType();

                if(dataType == t){
                    dataToNtVatiable.Add(t, nodeType);
                    return nodeType;
                }
            }

            Debug.Log("Not found??");
            return null;
        }

        [ContextMenu("Export")]
        public void Export(){
            Export(Application.dataPath + "/" + name + ".json");
        }

        public string ExportSerialized(){
            JSONImportExport jep = new JSONImportExport();
            return jep.Export(this); 
        }

        public void Export(string path){
            JSONImportExport jep = new JSONImportExport();
            jep.Export(this, path);      
        }

        [ContextMenu("Import")]
        public void Import(){
            sceneVariables.variableRepository.dictionary.OnAfterDeserialize();
            string path = Application.dataPath + "/" + name + ".json" ;
            Import(path);
        }

        public void Import(string path){
            JSONImportExport jimp = new JSONImportExport();
            NTGraph g = (NTGraph) jimp.Import(path);

            if(g == null) return;

            Clear();
            LoadFromGraph(g);            
        }

        public void ImportSerialized(string serialized){
            JSONImportExport jimp = new JSONImportExport();
            NTGraph g = (NTGraph) jimp.ImportSerialized(serialized);

            if(g == null) return;

            Clear();
            LoadFromGraph(g);  
        }

        public virtual void LoadFromGraph(NTGraph g){
            nodes = g.nodes;
            callbackNodes = g.callbackNodes;
            sceneVariables = g.sceneVariables;

            foreach(Node n in nodes){
                n.graph = this;
            }
        }
    }
}
