using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT{
    [System.Serializable]
    public class FlowNode : NTNode
    {
        [NTInput] public DummyConnection flowIn;
        
        [NTOutput] public DummyConnection flowOut;

        public override NodeExecutionContext NextNode(NodeExecutionContext context){
            
            NTNode node = GetNode(nameof(flowOut));
            NodePort port = GetPort(nameof(flowOut));

            Debug.Log("Next node ? " + node);


            return new NodeExecutionContext{node = node, inputPort = port?.Connection, outputPort = port};
        }

    }
}