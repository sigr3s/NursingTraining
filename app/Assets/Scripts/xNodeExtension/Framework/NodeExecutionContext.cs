

using NT.Nodes;
using XNode;

namespace NT
{
    public class NodeExecutionContext
    {
        public NTNode node;
        public NodePort inputPort; 

        public NodeExecutionContext(NTNode node, NodePort inputPort){
            this.node = node;
            this.inputPort = inputPort;
        }
    }
    
}