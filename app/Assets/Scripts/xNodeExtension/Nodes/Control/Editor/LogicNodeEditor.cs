using UnityEngine;
using UnityEditor;

using XNodeEditor;
using XNode;
using System.Collections.Generic;

namespace NT.Nodes.Control
{
    [CustomNodeEditor(typeof(LogicNode))]
    public class LogicNodeEditor : NodeEditor {

        public override void OnHeaderGUI() {
            GUIStyle errorStyle = new GUIStyle(GUI.skin.button);
            errorStyle.alignment = TextAnchor.MiddleCenter;
            errorStyle.fontStyle = FontStyle.Bold;
            errorStyle.normal.textColor = Color.white;

            NTNode node = (NTNode) target;

            EditorGUILayout.BeginHorizontal();
            
            base.OnHeaderGUI();

            if(node.hasError){
                Color originalColor = GUI.backgroundColor;

                GUI.backgroundColor = Color.red;
                EditorGUILayout.LabelField(new GUIContent("!!!!!!", node.error ), errorStyle, GUILayout.Width(50) );
                GUI.backgroundColor = originalColor;
            }

            EditorGUILayout.EndHorizontal();
        }


    }
}
