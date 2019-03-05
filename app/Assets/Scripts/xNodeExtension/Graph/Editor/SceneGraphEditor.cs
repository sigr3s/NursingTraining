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
			if(sceneTreeViewState == null){ InitializeSceneTree(); }

			Rect r = EditorGUILayout.BeginVertical(GUI.skin.button ,GUILayout.Width(200) );

				bool isFocued = r.Contains(Event.current.mousePosition);
				GUILayout.Label(Event.current.mousePosition.ToString() + " ___ " + isFocued);

				GUILayout.Label("Nodes", GUI.skin.button);

				Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
				nodeTreeView.OnGUI(rect);

				GUILayout.Label("Scene Items", GUI.skin.button);

				rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
				sceneTreeView.OnGUI(rect);

				GUILayout.Label("Variable", GUI.skin.button);

				rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
				variableTreeView.OnGUI(rect);

				if(GUILayout.Button("Reload")){
					ReloadSceneTree();
					ReloadVariableTree();
				}

			EditorGUILayout.EndVertical();

			//FIXME:
			//if(Event.current.isMouse){
				//Event.current.Use();
			//}

			HandleNodeMenu();
			HandleVariableMenu();
			HandleSceneMenu();
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

		private void InitializeVariablesTree(){
			if (variableTreeViewState == null)
				variableTreeViewState = new TreeViewState ();
			ReloadVariableTree();
		}

		private void ReloadVariableTree(){
			variablesID = 0;
			variableItems = new List<TreeViewItem>();

			SceneGraph t = target as SceneGraph;
			if(t.sceneVariables != null){
				NTVariableRepository repo = t.sceneVariables.variableRepository;
				for(int i = 0; i < repo.dictionary.keys.Count; i++){
					string variable = repo.dictionary.keys[i];
					string displayName = variable.Replace("NT.Variables.NT", "");
					NTVariableDictionary varDict = repo.dictionary.values[i];

					if(typeof(ISceneObject).IsAssignableFrom(varDict._dictType)){
						continue;
					}

					variableItems.Add( new TreeViewItem{id = variablesID,  depth = 0, displayName = displayName});
					variablesID++;

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

			variableTreeView = new VariableTreeView(variableTreeViewState, variableItems);
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
						t = typeof(GetNTVariableNode);
					}
					else
					{
						t = typeof(SetNTVariableNode);
					}

					if(t == null) return;

					currentVariableNode = ScriptableObject.CreateInstance(t) as NTNode;
					currentVariableNode.name = variableItem.vairbaleKey;
					currentVariableNode.graph = target as XNode.NodeGraph;
					IVariableNode ivn = (IVariableNode) currentVariableNode;
					ivn.SetNTVariableType(variableItem.variableType);
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

	
	#region Scene Objects
		TreeViewState sceneTreeViewState;
		VariableTreeView sceneTreeView;
		List<TreeViewItem> sceneItems = new List<TreeViewItem>();
		int sceneItemsID = 0;
		Node currentSceneNode = null;


		private void InitializeSceneTree(){
			if (sceneTreeViewState == null)
				sceneTreeViewState = new TreeViewState ();

			ReloadSceneTree();
		}

        private void ReloadSceneTree()
        {
			sceneItemsID = 0;
			sceneItems = new List<TreeViewItem>();

			SceneGraph t = target as SceneGraph;

			if(t.sceneVariables != null){
				NTVariableRepository repo = t.sceneVariables.variableRepository;
				for(int i = 0; i < repo.dictionary.keys.Count; i++){
					string variable = repo.dictionary.keys[i];
					NTVariableDictionary varDict = repo.dictionary.values[i];

					if(!typeof(ISceneObject).IsAssignableFrom(varDict._dictType)){
						continue;
					}					

					string displayName = variable.Replace("NT.SceneObjects.NT", "");

					sceneItems.Add( new TreeViewItem{id = sceneItemsID,  depth = 0, displayName = displayName});
					sceneItemsID++;

					for(int j = 0; j < varDict.keys.Count; j++){

						NTVariable ntv = varDict[varDict.keys[j]];

						var d = ReflectionUtilities.DeglosseInBasicTypes(ntv.GetDataType() );

						sceneItems.Add( new TreeViewItem{id = sceneItemsID,  depth = 1, displayName = varDict.keys[j]});
						sceneItemsID++;

						sceneItems.Add( new TreeViewItem{id = sceneItemsID,  depth = 2, displayName = "D"});
						sceneItemsID++;

						foreach (var item in d)
						{
							foreach(string var in item.Value){

								sceneItems.Add( new VariableTreeViewItem{id = sceneItemsID,  depth = 3, displayName =  "GET " + var, 
												vairbaleKey = varDict.keys[j], variableNodeType = VariableTreeViewItem.VariableNodeType.GET,
												variableType = varDict._dictType });
								sceneItemsID++;
							}
						}

						sceneItems.Add( new TreeViewItem{id = sceneItemsID,  depth = 2, displayName = "G"});
						sceneItemsID++;

						sceneItems.Add( new VariableTreeViewItem{id = sceneItemsID,  depth = 3, displayName =  "GET " +varDict.keys[j], 
											vairbaleKey = varDict.keys[j], variableNodeType = VariableTreeViewItem.VariableNodeType.GET,
											variableType = varDict._dictType });
						sceneItemsID++;

						sceneItems.Add( new VariableTreeViewItem{id = sceneItemsID,  depth = 3, displayName =  "SET " +varDict.keys[j],
											vairbaleKey = varDict.keys[j], variableNodeType = VariableTreeViewItem.VariableNodeType.SET,
											variableType = varDict._dictType });
						sceneItemsID++;
					}
				}
			}

			sceneTreeView = new VariableTreeView(sceneTreeViewState, sceneItems);
		}

		private void HandleSceneMenu(){
			Event e = Event.current;

			EventType eventType = e.rawType;
			VariableTreeViewItem variableItem = sceneTreeView.selectedItem as VariableTreeViewItem;


			if(variableItem != null){
				NodeEditorWindow.currentActivity = NodeEditorWindow.NodeActivity.Idle;

				if(currentSceneNode == null){
					Type t = null;

					if(variableItem.variableNodeType == VariableTreeViewItem.VariableNodeType.GET){
						t = typeof(GetNTVariableNode);
					}
					else
					{
						t = typeof(SetNTVariableNode);
					}

					if(t == null) return;

					currentSceneNode = ScriptableObject.CreateInstance(t) as NTNode;
					currentSceneNode.name = variableItem.vairbaleKey;
					currentSceneNode.graph = target as XNode.NodeGraph;
					IVariableNode ivn = (IVariableNode) currentSceneNode;
					ivn.SetNTVariableType(variableItem.variableType);
					ivn.SetVariableKey(variableItem.vairbaleKey);


					if(e.type != EventType.Repaint) return;
				}

				DrawNodePreview(e.mousePosition, currentSceneNode);
			}
			else
			{
				if(currentSceneNode != null){
					currentSceneNode.name = "variable";
					currentSceneNode.position = NodeEditorWindow.current.WindowToGridPosition(e.mousePosition);
					CopyNode(currentSceneNode);
				}

				currentSceneNode = null;
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