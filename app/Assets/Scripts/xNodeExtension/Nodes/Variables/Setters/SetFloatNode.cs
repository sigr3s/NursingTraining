using NT.Graph;

namespace NT.Nodes.Variables
{
    public class SetFloatNode : SetGenericVariableNode<float> {
        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            float value = GetInputValue<float>(nameof(this.value), this.value);
            g.sceneVariables.SetFloat(variableKey, value);
        }
    }
}