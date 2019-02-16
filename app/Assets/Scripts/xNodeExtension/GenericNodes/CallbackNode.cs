using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class CallbackNode : NTNode
    {
        [NTOutput] public DummyConnection flowOut;
        [SerializeField] public string callbackKey;
        public override object GetValue(NodePort port) {
            return null;
        }

        public override NodeExecutionContext NextNode(NodeExecutionContext context){
            return new NodeExecutionContext( GetNode(nameof(flowOut)), GetPort(nameof(flowOut)) );
        }
    }
}