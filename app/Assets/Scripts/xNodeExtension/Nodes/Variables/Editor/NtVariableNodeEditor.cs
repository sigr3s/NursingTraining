using XNode;
using NT.Atributes;
using XNodeEditor;
using UnityEngine;
using NT.Variables;
using UnityEditor;
using NT.Graph;
using System.Linq;

namespace NT.Nodes.Variables
{
    public class NTVariableNodeEditor<T> : NodeEditor where T: INTVaribale{
         public override void OnBodyGUI() {
            NTNode node = target as NTNode;
            IVariableNode ivn = target as IVariableNode;
            NTGraph graph =  node.graph as NTGraph;
            
            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.variableRepository.GetOptions<T>(ivn.GetVariableKey(), out _choiceIndex).ToArray();
            
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                ivn.SetVariableKey(_choices[_newChoiceIndex]);
            }

            serializedObject.Update();
            
            string[] excludes = { "m_Script", "graph", "position", "ports" };
            SerializedProperty iterator = serializedObject.GetIterator();

            bool enterChildren = true;
            EditorGUIUtility.labelWidth = 84;
            while (iterator.NextVisible(enterChildren)) {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                //EditorGUILayout.PropertyField(iterator, GUILayout.MinWidth(30));
            }
            serializedObject.ApplyModifiedProperties();


            base.OnBodyGUI();
        }

        public override Color GetTint(){
            NTNode node = target as NTNode;

            if(node.isExecuting){
                return Color.red;
            }
            else
            {
                NTGraph graph =  node.graph as NTGraph;
                return graph.sceneVariables.GetColorFor(typeof(T));
            }
        }  
    }
    
}