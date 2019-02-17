using XNode;
using NT.Atributes;

namespace NT.Nodes.Variables{
    [System.Serializable]
    public class GenericVariableNode<T> : NTNode
    {
        [NTOutput] public T variable;
        public GenericVariable<T> internalVariable;
        public override object GetValue(NodePort port) {
            return internalVariable;
        }
    }
}