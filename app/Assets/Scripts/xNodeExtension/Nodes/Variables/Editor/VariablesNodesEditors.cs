using XNode;
using NT.Atributes;
using XNodeEditor;
using UnityEngine;
using NT.Variables;
using UnityEditor;
using NT.Graph;

namespace NT.Nodes.Variables{

    [CustomNodeEditor(typeof(StringNode))]
    public class StringNodeEditor  : NodeEditor {
        public override Color GetTint(){
            return VariablesColors.StringColor;
        }

        public override void OnBodyGUI() {
            StringNode node = target as StringNode;
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

    [CustomNodeEditor(typeof(FloatNode))]
    public class FloatNodeEditor  : NodeEditor {
        public override Color GetTint(){
            return VariablesColors.FloatColor;
        }

        public override void OnBodyGUI() {
            FloatNode node = target as FloatNode;
            NTGraph graph =  node.graph as NTGraph;
            
            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.GetFloatOptions(node.variableKey, out _choiceIndex).ToArray();
            
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                node.variableKey = _choices[_newChoiceIndex];
            }

            base.OnBodyGUI();
        }
    }

    [CustomNodeEditor(typeof(IntegerNode))]
    public class IntegerNodeEditor  : NodeEditor {
        public override Color GetTint(){
            return VariablesColors.IntegerColor;
        }

        public override void OnBodyGUI() {
            IntegerNode node = target as IntegerNode;
            NTGraph graph =  node.graph as NTGraph;
            
            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.GetIntegerOptions(node.variableKey, out _choiceIndex).ToArray();
            
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                node.variableKey = _choices[_newChoiceIndex];
            }

            base.OnBodyGUI();
        }
    }

    [CustomNodeEditor(typeof(BooleanNode))]
    public class BooleanNodeEditor  : NodeEditor {
        public override Color GetTint(){
            return VariablesColors.BooleanColor;
        }

        public override void OnBodyGUI() {
            BooleanNode node = target as BooleanNode;
            NTGraph graph =  node.graph as NTGraph;
            
            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.GetBoolOptions(node.variableKey, out _choiceIndex).ToArray();
            
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                node.variableKey = _choices[_newChoiceIndex];
            }

            base.OnBodyGUI();
        }
    }
}