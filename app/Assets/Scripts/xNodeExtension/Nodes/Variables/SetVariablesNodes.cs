using NT.Graph;

namespace NT.Nodes.Variables{
    public class SetStringNode : SetGenericVariableNode<string> {
        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            string value = GetInputValue<string>(nameof(this.value), this.value);
            g.sceneVariables.SetString(variableKey, value);
        }
    }

    public class SetFloatNode : SetGenericVariableNode<float> {
        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            float value = GetInputValue<float>(nameof(this.value), this.value);
            g.sceneVariables.SetFloat(variableKey, value);
        }
    }

    public class SetIntegerNode : SetGenericVariableNode<int> {
        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            int value = GetInputValue<int>(nameof(this.value), this.value);
            g.sceneVariables.SetInteger(variableKey, value);
        }
    }

    public class SetBooleanNode : SetGenericVariableNode<bool> {
        public override void ExecuteNode(){
            NTGraph g = graph as NTGraph;
            bool value = GetInputValue<bool>(nameof(this.value), this.value);
            g.sceneVariables.SetBool(variableKey, value);
        }
    }
}