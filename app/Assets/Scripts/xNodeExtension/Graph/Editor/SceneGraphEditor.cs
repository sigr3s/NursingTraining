using System;
using System.Collections;
using System.Collections.Generic;
using NT.Variables;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using XNode.Examples.StateGraph;
using XNodeEditor;
using XNode;
using NT.Nodes.Control;
using System.Runtime.Serialization;
using NT.Nodes.Variables;

namespace NT.Graph
{
    [NodeGraphEditor.CustomNodeGraphEditor(typeof(SceneGraph))]
	public class SceneGraphEditor : NodeGraphEditor {
		public readonly string selectedNamespace = "NT.Nodes";
		public readonly string replaceNamespace = "NT";

	#region Graph Override
		public override string GetNodeMenuName(System.Type type) {
			if (type.Namespace.Contains(selectedNamespace) ) {
				if(type.IsGenericType) return null;

				string menu =  base.GetNodeMenuName(type).Replace(selectedNamespace.Replace(".", "/"), replaceNamespace);
				Debug.Log("YES?  " + menu);

				return menu;
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
			if(nodeTreeViewState == null){ InitializeNodesTree(); }
			if(variableTreeViewState == null){ InitializeVariablesTree(); }

			Rect r = EditorGUILayout.BeginVertical(GUI.skin.button ,GUILayout.Width(Screen.width/8) );

				bool isFocued = r.Contains(Event.current.mousePosition);
				GUILayout.Label(Event.current.mousePosition.ToString() + " ___ " + isFocued);

				GUILayout.Label("Nodes", GUI.skin.button);

				Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
				nodeTreeView.OnGUI(rect);

				GUILayout.Label("Scene Items", GUI.skin.button);

				rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
				variableTreeView.OnGUI(rect);

				GUILayout.Label("Variable", GUI.skin.button);

				rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
				variableTreeView.OnGUI(rect);

				if(GUILayout.Button("Add variable")){

				}

			EditorGUILayout.EndVertical();

			if(Event.current.isMouse){
				NodeEditorWindow.current.hasFocus = !isFocued;
			}

			HandleNodeMenu();
			HandleVariableMenu();
		}
	#endregion

	#region Nodes Menu
		[SerializeField] TreeViewState nodeTreeViewState;
		NodesTreeView nodeTreeView;
		private List<string> LastParts = new List<string>();
		List<TreeViewItem> nodeItems = new List<TreeViewItem>();
        int nodesID = 0;
		Node currentnode = null;


		public NodeTreeViewItem GetNodeTreeViewItem(System.Type type){
			if (type.Namespace.Contains(selectedNamespace) ) {
				if(type.IsGenericType) return null;
				if(type.IsCastableTo(typeof(IVariableNode)) ) return null;
				string menu =  base.GetNodeMenuName(type).Replace(selectedNamespace.Replace(".", "/"), replaceNamespace + "/");
				var parts = menu.Split('/');
				int depth = 0;

				for(int i = 0; i < parts.Length; i++){
					if(i == 0 || string.IsNullOrEmpty(parts[i])){ continue; }

					if(i == parts.Length - 1 ){
						nodesID++;
						return new NodeTreeViewItem{id = nodesID, depth = depth, displayName = parts[i], nodeType = type};
					}
					else
					{
						if(LastParts.Count < depth+1){
							nodesID++;
							LastParts.Add(parts[i]);
							nodeItems.Add(new TreeViewItem{id = nodesID,  depth = depth, displayName = parts[i]});
						}
						else
						{
							if(LastParts[depth] != parts[i]){
								LastParts[depth] = parts[i];
								nodesID++;
								nodeItems.Add(new TreeViewItem{id = nodesID,  depth = depth, displayName = parts[i]});
							}
						}

						depth++;
					}
				}
			}

			return null;
		}

		private void HandleNodeMenu(){
            Event e = Event.current;

			EventType eventType = e.rawType;
			NodeTreeViewItem nodeItem = nodeTreeView.selectedItem as NodeTreeViewItem;

			if(nodeItem != null){
				NodeEditorWindow.currentActivity = NodeEditorWindow.NodeActivity.Idle;

				if(currentnode == null){
					Type t = nodeItem.nodeType;
					currentnode = ScriptableObject.CreateInstance(t) as Node;
					currentnode.name = nodeItem.displayName;
					currentnode.graph = target as XNode.NodeGraph;

					if(e.type != EventType.Repaint) return;
				}

				DrawNodePreview(e.mousePosition, currentnode);
			}
			else
			{
				if(currentnode != null){
					CreateNode(currentnode.GetType(), NodeEditorWindow.current.WindowToGridPosition(e.mousePosition) );
				}

				currentnode = null;
			}
        }

		private void InitializeNodesTree()
        {
			for (int i = 0; i < NodeEditorWindow.nodeTypes.Length; i++) {
                Type type = NodeEditorWindow.nodeTypes[i];
                TreeViewItem tvwi = GetNodeTreeViewItem(type);
                if (tvwi == null) continue;

				nodeItems.Add(tvwi);
            }


			if (nodeTreeViewState == null)
				nodeTreeViewState = new TreeViewState ();

			nodeTreeView = new NodesTreeView(nodeTreeViewState, nodeItems);
        }

        private void DrawNodePreview(Vector2 position, Node node){
			Color guiColor = GUI.color;

			NodeEditor nodeEditor = NodeEditor.GetEditor(node);

			NodeEditor.portPositions = new Dictionary<XNode.NodePort, Vector2>();

			GUILayout.BeginArea(new Rect(position, new Vector2(nodeEditor.GetWidth(), 4000)));

				GUIStyle style = new GUIStyle(nodeEditor.GetBodyStyle());
				GUIStyle highlightStyle = new GUIStyle(NodeEditorResources.styles.nodeHighlight);
				highlightStyle.padding = style.padding;
				style.padding = new RectOffset();
				GUI.color = nodeEditor.GetTint();

				GUILayout.BeginVertical(style);
					GUI.color = NodeEditorPreferences.GetSettings().highlightColor;
					GUILayout.BeginVertical(new GUIStyle(highlightStyle));

						GUI.color =  guiColor;

						nodeEditor.OnHeaderGUI();
						nodeEditor.OnBodyGUI();

					GUILayout.EndVertical();

				GUILayout.EndVertical();

			GUILayout.EndArea();
		}

	#endregion

	#region Variables Menu
		TreeViewState variableTreeViewState;
		VariableTreeView variableTreeView;
		List<TreeViewItem> variableItems = new List<TreeViewItem>();
		int variablesID = 0;

		Node currentVariableNode = null;

		Dictionary<Type, Type> setNodes = new Dictionary<Type, Type>();
		Dictionary<Type, Type> getNodes = new Dictionary<Type, Type>();

		private void InitializeVariablesTree(){
			if (variableTreeViewState == null)
				variableTreeViewState = new TreeViewState ();

			for (int i = 0; i < NodeEditorWindow.nodeTypes.Length; i++) {
                Type nodeType = NodeEditorWindow.nodeTypes[i];

				if(nodeType.IsGenericType) continue;

				if(IsSubclassOfRawGeneric(typeof(SetNTVariableNode<,>), nodeType) ){
					if(nodeType.BaseType.IsGenericType) {
						Type setNodeVariableType = nodeType.BaseType.GetGenericArguments()[1];
						setNodes.Add(setNodeVariableType, nodeType);
					}
				}

				if(IsSubclassOfRawGeneric(typeof(GetNTVariableNode<,>), nodeType)){
					if(nodeType.BaseType.IsGenericType) {
						Type setNodeVariableType = nodeType.BaseType.GetGenericArguments()[1];
						getNodes.Add(setNodeVariableType, nodeType);
					}
				}
            }

			ReloadVariableTree();

			variableTreeView = new VariableTreeView(variableTreeViewState, variableItems);
		}

		private void ReloadVariableTree(){
			variablesID = 0;
			variableItems = new List<TreeViewItem>();

			SceneGraph t = target as SceneGraph;
			if(t.sceneVariables != null){
				NTVariableRepository repo = t.sceneVariables.variableRepository;
				for(int i = 0; i < repo.dictionary.keys.Count; i++){
					string variable = repo.dictionary.keys[i];
					variableItems.Add( new TreeViewItem{id = variablesID,  depth = 0, displayName = variable.Replace("NT.Variables.NT", "")});
					variablesID++;

					NTVariableDictionary varDict = repo.dictionary.values[i];
					for(int j = 0; j < varDict.keys.Count; j++){

						variableItems.Add( new TreeViewItem{id = variablesID,  depth = 1, displayName = varDict.keys[j]});
						variablesID++;

						variableItems.Add( new VariableTreeViewItem{id = variablesID,  depth = 2, displayName =  "GET " +varDict.keys[j], 
											vairbaleKey = varDict.keys[j], variableNodeType = VariableTreeViewItem.VariableNodeType.GET,
											variableType = varDict._dictType });
						variablesID++;

						variableItems.Add( new VariableTreeViewItem{id = variablesID,  depth = 2, displayName =  "SET " +varDict.keys[j],
											vairbaleKey = varDict.keys[j], variableNodeType = VariableTreeViewItem.VariableNodeType.SET,
											variableType = varDict._dictType });
						variablesID++;
					}
				}
			}
		}

		private void HandleVariableMenu(){
			Event e = Event.current;

			EventType eventType = e.rawType;
			VariableTreeViewItem variableItem = variableTreeView.selectedItem as VariableTreeViewItem;

			if(variableItem != null){
				NodeEditorWindow.currentActivity = NodeEditorWindow.NodeActivity.Idle;

				if(currentVariableNode == null){
					Type t = null;

					if(variableItem.variableNodeType == VariableTreeViewItem.VariableNodeType.GET){
						if(getNodes.ContainsKey(variableItem.variableType)){
							t = getNodes[variableItem.variableType];
						}
					}
					else
					{
						if(setNodes.ContainsKey(variableItem.variableType)){
							t = setNodes[variableItem.variableType];
						}
					}

					if(t == null) return;

					currentVariableNode = ScriptableObject.CreateInstance(t) as NTNode;
					currentVariableNode.name = variableItem.vairbaleKey;
					currentVariableNode.graph = target as XNode.NodeGraph;
					IVariableNode ivn = (IVariableNode) currentVariableNode;
					ivn.SetVariableKey(variableItem.vairbaleKey);


					if(e.type != EventType.Repaint) return;
				}

				DrawNodePreview(e.mousePosition, currentVariableNode);
			}
			else
			{
				if(currentVariableNode != null){
					currentVariableNode.name = "variable";
					currentVariableNode.position = NodeEditorWindow.current.WindowToGridPosition(e.mousePosition);
					CopyNode(currentVariableNode);
				}

				currentVariableNode = null;
			}
		}

	#endregion

	#region Utility
		static bool IsSubclassOfRawGeneric(Type generic, Type toCheck) {
			while (toCheck != null && toCheck != typeof(object)) {
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur) {
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}
	#endregion

    }
}