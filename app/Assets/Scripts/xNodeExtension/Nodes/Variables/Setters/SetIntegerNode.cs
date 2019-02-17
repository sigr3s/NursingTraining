using NT.Graph;

namespace NT.Nodes.Variables
{
    public class SetIntegerNode : SetGenericVariableNode<int> {
        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            int value = GetInputValue<int>(nameof(this.value), this.value);
            g.sceneVariables.SetInteger(variableKey, value);
        }
    }
}