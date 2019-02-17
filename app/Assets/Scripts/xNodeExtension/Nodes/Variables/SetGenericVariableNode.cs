using NT.Atributes;
using XNode;

namespace NT.Nodes.Variables{
    [System.Serializable]
    public class SetGenericVariableNode<T> : NTNode
    {
        [NTInput] public T variable;

    }
}