using NT.Graph;

namespace NT.Nodes.Variables
{
    public class SetStringNode : SetGenericVariableNode<string> {
        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            string value = GetInputValue<string>(nameof(this.value), this.value);
            g.sceneVariables.SetString(variableKey, value);
        }
    }
}