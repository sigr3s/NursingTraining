using System;
using NT.Atributes;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class NTNode : Node
    {
        [NTOutput] public DummyConnection flowOut;
    }
}