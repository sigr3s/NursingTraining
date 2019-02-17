using NT.Graph;

namespace NT.Nodes.Variables
{
    public class SetBooleanNode : SetGenericVariableNode<bool> {
        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            bool value = GetInputValue<bool>(nameof(this.value), this.value);
            g.sceneVariables.SetBool(variableKey, value);
        }
    }
}