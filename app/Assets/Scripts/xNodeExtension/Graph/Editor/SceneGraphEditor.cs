using System;
using System.Collections;
using System.Collections.Generic;
using NT.Variables;
using UnityEngine;
using XNode.Examples.StateGraph;
using XNodeEditor;

namespace NT.Graph {
	[NodeGraphEditor.CustomNodeGraphEditor(typeof(SceneGraph))]
	public class SceneGraphEditor : NodeGraphEditor {

		public override string GetNodeMenuName(System.Type type) {
			if (type.Namespace.Contains("NT.Nodes") ) {
				if(type.IsGenericType) return null;
								
				return base.GetNodeMenuName(type).Replace("NT/Nodes/", "NT/");
			} else return null;
		}

		public override Color GetTypeColor(Type type) {
			if(type == typeof(string)) return VariablesColors.StringColor;
			if(type == typeof(float)) return VariablesColors.FloatColor;
			if(type == typeof(int)) return VariablesColors.IntColor;
			if(type == typeof(bool)) return VariablesColors.BoolColor;
			if(type == typeof(DummyConnection) ) return Color.white;

            return NodeEditorPreferences.GetTypeColor(type);
        }
		public override void OnGUI(){
			
		}
	}
}