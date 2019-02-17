
using NT.Graph;
using XNode;

namespace NT.Nodes.Variables
{
    public class StringNode : GenericVariableNode<string> {
        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            return g.sceneVariables.GetString(variableKey);
        }
    }
}