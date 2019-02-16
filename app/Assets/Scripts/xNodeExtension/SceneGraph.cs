using System;
using System.Collections;
using System.Collections.Generic;
using NT.Nodes;
using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

namespace NT.Graph{


    [CreateAssetMenu(fileName = "New Scene Graph", menuName = "NT/Scene Graph")]
    public class SceneGraph : NodeGraph
    {
        public NTNode current;

        public List<CallbackNode> callbackNodes;

        public Dictionary<string, List<CallbackNode>> callbackNodesDict;
        public List<NTNode> executionNodes;


        private CoroutineRunner coroutineRunner;

        public override Node AddNode(System.Type type){
            Node node = base.AddNode(type);
            if(node is CallbackNode){
                callbackNodes.Add( (CallbackNode) node );
            }
            return node;
        }

        [ContextMenu("Start Execution")]
        public void StartExecution(){
            MessageSystem.onMessageSent -= MessageRecieved;
            MessageSystem.onMessageSent += MessageRecieved;

            if(coroutineRunner == null){
                GameObject go = new GameObject("Coroutine Executor");
                coroutineRunner =  go.AddComponent<CoroutineRunner>();
            }

            GenerateCallbackDict();
            MessageSystem.SendMessage("start");

        }

        private void GenerateCallbackDict()
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

        private void MessageRecieved(string message)
        {
            Debug.Log("Message recieved!   " + message);
            if(!string.IsNullOrEmpty(message) && callbackNodesDict.ContainsKey(message)){
                List<CallbackNode> nodesToExecute = callbackNodesDict[message];
                foreach(CallbackNode cn in nodesToExecute){
                    coroutineRunner.StartCoroutine(StartExecutionFlow(cn));
                }
            }
        }

        private IEnumerator StartExecutionFlow(CallbackNode callbackNode)
        {
            NodeExecutionContext nodeExecutionContext = new NodeExecutionContext(callbackNode, null);

            while(nodeExecutionContext.node != null){

                Debug.Log("Execute node:  " + nodeExecutionContext.node.name);
                nodeExecutionContext.node.Enter();

                yield return new YieldNode(nodeExecutionContext.node);
                
                Debug.Log("Finished node:  " + nodeExecutionContext.node.name);

                yield return new WaitForSeconds(1.25f);

                Debug.Log("Finished waiting?:  " + nodeExecutionContext.node.name);

                nodeExecutionContext.node.Exit();
                

                nodeExecutionContext = nodeExecutionContext.node.NextNode(nodeExecutionContext);
            }

            yield return null;
        }
    }
}