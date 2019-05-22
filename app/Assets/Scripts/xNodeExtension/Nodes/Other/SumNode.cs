using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes.Other{

    [System.Serializable]
    public class SumNode : FlowNode
    {
        [NTInput] public float val1;
        [NTInput] public float val2;

        [NTOutput] public float result;


        public override object GetValue(NodePort port) {
            float val1 = GetInputValue<float>(nameof(val1), this.val1);
            float val2 = GetInputValue<float>(nameof(val2), this.val2);

            result = val1 + val2;

            return result;
        }

        public override string GetDisplayName(){
            return "Sum";
        }

        
    }   

}