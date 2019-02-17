using System;
using NT.Atributes;
using UnityEngine;

namespace NT.Nodes.Messages{
    [System.Serializable]
    public class SendMessageNode : FlowNode
    {
        [SerializeField] public string message;

        private bool sentMessage = false;
        
        public override NodeExecutionContext NextNode(NodeExecutionContext context){
            return new NodeExecutionContext( GetNode(nameof(flowOut)), GetPort(nameof(flowOut)) );
        }

        public override void ExecuteNode(){
            sentMessage = false;
            string message = GetInputValue<string>(nameof(this.message), this.message);
            MessageSystem.SendMessage(message);
            sentMessage = true;
        }

        public override bool KeepWaiting(){
            return !sentMessage;
        }
    }
}