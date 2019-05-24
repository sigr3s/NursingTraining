using System;
using System.Collections;
using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes.Messages{
    [System.Serializable]
    public class SendMessageNode : FlowNode
    {
        [SerializeField] public string message;

        public override NodeExecutionContext NextNode(NodeExecutionContext context){
            NTNode node = GetNode(nameof(flowOut));
            NodePort port = GetPort(nameof(flowOut));

            return new NodeExecutionContext{node = node, inputPort = port?.Connection, outputPort = port};
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context){
            string message = GetInputValue<string>(nameof(this.message), this.message);
            MessageSystem.SendMessage(message);

            yield return null;
        }

        public override string GetDisplayName(){
            return "Send Message";
        }
    }
}