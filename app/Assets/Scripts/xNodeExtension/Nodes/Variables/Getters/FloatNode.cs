
using NT.Graph;
using XNode;

namespace NT.Nodes.Variables
{
    public class FloatNode : GenericVariableNode<float> {
        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            return g.sceneVariables.GetFloat(variableKey);
        }
    }
}