
using NT.Graph;
using XNode;

namespace NT.Nodes.Variables
{
    public class IntegerNode : GenericVariableNode<int> {
        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            return g.sceneVariables.GetInteger(variableKey);
        }
    }
}