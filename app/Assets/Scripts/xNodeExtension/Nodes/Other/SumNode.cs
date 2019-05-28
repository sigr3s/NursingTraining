using NT.Atributes;
using UnityEngine;
using XNode;

namespace NT.Nodes.Other{

    [System.Serializable]
    public class SumNode : NTNode
    {
        [NTInput] public float valueA;
        [NTInput] public float valueB;

        [NTOutput] public float result;


        public override object GetValue(NodePort port) {
            float val1 = GetInputValue<float>(nameof(valueA), this.valueA);
            float val2 = GetInputValue<float>(nameof(valueB), this.valueB);

            result = val1 + val2;

            return result;
        }

        public override string GetDisplayName(){
            return "Sum";
        }

        
    }   

}