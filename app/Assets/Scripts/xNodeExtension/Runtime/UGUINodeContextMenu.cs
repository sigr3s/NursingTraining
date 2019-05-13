using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUINodeContextMenu : UGUIContextMenu {

	public Action<Type, Vector2> onClickSpawn;
	public Action onGroupClick;


	public void SpawnNode(Type nodeType) {
		Vector2 pos = new Vector2(transform.localPosition.x, -transform.localPosition.y);
		onClickSpawn(nodeType, pos);
	}

	public void RemoveNode() {
		RuntimeGraph runtimeMathGraph = GetComponentInParent<RuntimeGraph>();

		selected.Remove();
		runtimeMathGraph.Refresh();
		Close();
	}

	public void GroupNodes(){
		RuntimeGraph runtimeMathGraph = GetComponentInParent<RuntimeGraph>();
		runtimeMathGraph.GroupSelected();
		runtimeMathGraph.Refresh();
		Close();
	}
}  