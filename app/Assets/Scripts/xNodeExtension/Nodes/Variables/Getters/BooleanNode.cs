
using NT.Graph;
using XNode;

namespace NT.Nodes.Variables
{
    public class BooleanNode : GenericVariableNode<bool> {
        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            return g.sceneVariables.GetBool(variableKey);
        }
    }
}