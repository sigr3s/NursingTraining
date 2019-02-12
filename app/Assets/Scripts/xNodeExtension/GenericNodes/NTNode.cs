using XNode;

namespace NT.Nodes{
    [System.Serializable]
    public class NTNode : Node
    {
        [Output] public Node flowOut;
    }
}