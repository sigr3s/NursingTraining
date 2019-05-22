using System;
using System.Collections.Generic;
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
		RuntimeGraph runtimeGraph = GetComponentInParent<RuntimeGraph>();

		if(runtimeGraph.selectedNodes.Count > 0){

			for(int i = runtimeGraph.selectedNodes.Count -1 ; i >= 0; i --){
				IUGUINode n = runtimeGraph.selectedNodes[i];
				n.RemoveNode();
			}

			runtimeGraph.selectedNodes = new List<IUGUINode>();
		}
		else
		{
			selected.Remove();
		}

		runtimeGraph.Refresh();
		Close();
	}

	public void GroupNodes(){
		RuntimeGraph runtimeGraph = GetComponentInParent<RuntimeGraph>();
		runtimeGraph.GroupSelected();
		runtimeGraph.Refresh();
		Close();
	}
}  