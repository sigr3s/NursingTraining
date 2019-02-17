
using NT.Graph;
using XNode;

namespace NT.Nodes.Variables{
    public class StringNode : GenericVariableNode<string> {
        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            return g.sceneVariables.GetString(variableKey);
        }
    }

    public class FloatNode : GenericVariableNode<float> {
        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            return g.sceneVariables.GetFloat(variableKey);
        }
    }

    public class IntegerNode : GenericVariableNode<int> {
        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            return g.sceneVariables.GetInteger(variableKey);
        }
    }

    public class BooleanNode : GenericVariableNode<bool> {
        public override object GetValue(NodePort port) {
            NTGraph g = graph as NTGraph;
            return g.sceneVariables.GetBool(variableKey);
        }
    }
}