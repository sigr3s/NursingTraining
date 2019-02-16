using NT.Atributes;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class FlowNode : NTNode
    {
        [NTInput] public DummyConnection flowIn;
        
        [NTOutput] public DummyConnection flowOut;

        public override NodeExecutionContext NextNode(NodeExecutionContext context){

            return new NodeExecutionContext( GetNode(nameof(flowOut)), GetPort(nameof(flowOut)) );
        }

    }
}