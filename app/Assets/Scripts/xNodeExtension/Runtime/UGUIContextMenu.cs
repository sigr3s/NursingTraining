using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

public class UGUIContextMenu : MonoBehaviour, IPointerExitHandler {

	public Action<Type, Vector2> onClickSpawn;
	public Action onGroupClick;
	public CanvasGroup group;
	[HideInInspector] public Node selectedNode;
	private Vector2 pos;

	private void Start() {
		Close();
		Initialize();
	}

	public virtual void Initialize(){

	}

	public void OpenAt(Vector2 pos) {
		transform.position = pos;
		group.alpha = 1;
		group.interactable = true;
		group.blocksRaycasts = true;
		transform.SetAsLastSibling();
	}

	public void Close() {
		group.alpha = 0;
		group.interactable = false;
		group.blocksRaycasts = false;
	}

	public void SpawnNode(Type nodeType) {
		Vector2 pos = new Vector2(transform.localPosition.x, -transform.localPosition.y);
		onClickSpawn(nodeType, pos);
	}

	public void RemoveNode() {
		RuntimeGraph runtimeMathGraph = GetComponentInParent<RuntimeGraph>();
		runtimeMathGraph.graph.RemoveNode(selectedNode);
		runtimeMathGraph.Refresh();
		Close();
	}

	public void GroupNodes(){
		RuntimeGraph runtimeMathGraph = GetComponentInParent<RuntimeGraph>();
		runtimeMathGraph.GroupSelected();
		runtimeMathGraph.Refresh();
		Close();
	}

	public void OnPointerExit(PointerEventData eventData) {
		Close();
	}
}