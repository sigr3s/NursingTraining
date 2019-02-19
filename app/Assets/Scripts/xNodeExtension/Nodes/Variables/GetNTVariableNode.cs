using XNode;
using NT.Atributes;
using NT.Variables;
using NT.Graph;
using UnityEngine;

namespace NT.Nodes.Variables
{
    [System.Serializable]
    public class GetNTVariableNode<T,K> : NTNode, IVariableNode where K: INTVaribale
    {
        [NTOutput] public T variable;
        [HideInInspector] public string variableKey;


         public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            T ntVariable = (T) g.sceneVariables.variableRepository.GetValue<K>(variableKey);

            if(ntVariable != null){
                return ntVariable;
            }

            return default(T);
        }

        public string GetVariableKey()
        {
            return variableKey;
        }

        public void SetVariableKey(string v)
        {
            variableKey = v;
        }
    }
}