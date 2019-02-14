using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode.Examples.StateGraph;
using XNodeEditor;

namespace NT.Graph {
	[NodeGraphEditor.CustomNodeGraphEditor(typeof(SceneGraph))]
	public class SceneGraphEditor : NodeGraphEditor {

		public override string GetNodeMenuName(System.Type type) {
			if (type.Namespace == "NT.Nodes"  || true) {
				return base.GetNodeMenuName(type).Replace("NT/Nodes/", "NT/");
			} else return null;
		}
	}
}