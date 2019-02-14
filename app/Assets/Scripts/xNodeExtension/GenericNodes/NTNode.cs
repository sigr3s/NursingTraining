using System;
using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class NTNode : Node
    {
        public virtual NTNode NextNode(){
            return null;
        }

        public virtual void ExecuteNode(){

        }

        public virtual bool Completed(){
            return true;
        }

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