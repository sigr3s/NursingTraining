using XNode;
using NT.Atributes;
using XNodeEditor;
using UnityEngine;
using NT.Variables;
using UnityEditor;
using NT.Graph;

namespace NT.Nodes.Variables{

    [CustomNodeEditor(typeof(SetStringNode))]
    public class SetStringNodeEditor  : NodeEditor {
        public override Color GetTint(){
            return VariablesColors.StringColor;
        }

        public override void OnBodyGUI() {
            SetStringNode node = target as SetStringNode;
            NTGraph graph =  node.graph as NTGraph;
            
            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.GetStringOptions(node.variableKey, out _choiceIndex).ToArray();
            
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                node.variableKey = _choices[_newChoiceIndex];
            }

            base.OnBodyGUI();
        }        
    }

    [CustomNodeEditor(typeof(SetFloatNode))]
    public class SetFloatNodeEditor  : NodeEditor {
        public override Color GetTint(){
            return VariablesColors.FloatColor;
        }

        public override void OnBodyGUI() {
            SetFloatNode node = target as SetFloatNode;
            NTGraph graph =  node.graph as NTGraph;
            
            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.GetStringOptions(node.variableKey, out _choiceIndex).ToArray();
            
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                node.variableKey = _choices[_newChoiceIndex];
            }

            base.OnBodyGUI();
        }        
    }

    [CustomNodeEditor(typeof(SetIntegerNode))]
    public class SetIntegerNodeEditor  : NodeEditor {
        public override Color GetTint(){
            return VariablesColors.IntegerColor;
        }

        public override void OnBodyGUI() {
            SetIntegerNode node = target as SetIntegerNode;
            NTGraph graph =  node.graph as NTGraph;
            
            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.GetStringOptions(node.variableKey, out _choiceIndex).ToArray();
            
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                node.variableKey = _choices[_newChoiceIndex];
            }

            base.OnBodyGUI();
        }        
    }

    [CustomNodeEditor(typeof(SetBooleanNode))]
    public class SetBooleanNodeEditor  : NodeEditor {
        public override Color GetTint(){
            return VariablesColors.BooleanColor;
        }

        public override void OnBodyGUI() {
            SetBooleanNode node = target as SetBooleanNode;
            NTGraph graph =  node.graph as NTGraph;
            
            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.GetStringOptions(node.variableKey, out _choiceIndex).ToArray();
            
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                node.variableKey = _choices[_newChoiceIndex];
            }

            base.OnBodyGUI();
        }        
    }
}