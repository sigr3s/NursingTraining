using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class CallabackNode : NTNode
    {
        [NTOutput] public DummyConnection flowOut;
        public override object GetValue(NodePort port) {
            return null;
        }

        public override NTNode NextNode(){
            return GetNode(nameof(flowOut));
        }
    }
}