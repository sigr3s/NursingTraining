using NT.Atributes;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class FlowNode : NTNode
    {
        [NTInput] public DummyConnection flowIn;
        
        [NTOutput] public DummyConnection flowOut;

    }
}