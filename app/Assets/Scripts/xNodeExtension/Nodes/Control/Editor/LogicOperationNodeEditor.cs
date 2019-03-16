using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;
using System.Collections.Generic;

namespace NT.Nodes.Control
{
    [CustomNodeEditor(typeof(LogicOperationNode))]
    public class LogicOperationNodeEditor : NodeEditor {
        

         public override void OnBodyGUI() {
            base.OnBodyGUI();

            LogicOperationNode node = (LogicOperationNode) target;

            if(node.extraConditions == null){
                node.extraConditions = new List<string>();
            }


            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Add")){
                string portName = "Extra"+node.extraConditions.Count;
                node.AddInstanceInput(typeof(bool) , Node.ConnectionType.Override, Node.TypeConstraint.Strict, portName);
                node.extraConditions.Add(portName);
            }

            if(GUILayout.Button("Remove")){
                if(node.extraConditions.Count > 0){
                    int lastIdx = node.extraConditions.Count - 1;
                    string portToRemove = node.extraConditions[lastIdx];

                    if(node.HasPort(portToRemove)){
                        node.RemoveInstancePort(portToRemove);
                        node.extraConditions.RemoveAt(lastIdx);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
         }
    }
}