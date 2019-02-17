using System;
using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT{
    [System.Serializable]
    public class NTNode : Node
    {

        public virtual NodeExecutionContext NextNode(NodeExecutionContext context){
            return null;
        }

        public virtual void ExecuteNode(){

        }

        public virtual bool KeepWaiting(){
            return false;
        }

        public virtual void Enter(){
            isExecuting = true;
        }

        public virtual void Exit(){
            isExecuting = false;
        }

        [HideInInspector] public bool isExecuting = false;
        public NTNode GetNode(string portName){
            NodePort nodePort = GetOutputPort(portName);

            if (!nodePort.IsConnected) {
				Debug.LogWarning("Node isn't connected");
				return null;
			}

			NTNode node = nodePort.Connection.node as NTNode;
            
            return node;
        }
    }
}