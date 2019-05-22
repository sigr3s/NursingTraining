using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeDrag : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private Vector3 offset;
	private IUGUINode node;

	private void Awake() {
		node = GetComponentInParent<IUGUINode>();
	}

	public void OnDrag(PointerEventData eventData) {

		if(node.GetRuntimeGraph().selectedNodes.Count > 0 && node.GetRuntimeGraph().selectedNodes.Contains(node)){
			Vector3 oldLocalPos = node.GetGameObject().transform.localPosition;
			node.GetGameObject().transform.localPosition = node.GetRuntimeGraph().scrollRect.content.InverseTransformPoint(eventData.position) - offset;
			Vector3 o = oldLocalPos  - node.GetGameObject().transform.localPosition;

			foreach (var item in node.GetRuntimeGraph().selectedNodes)
			{
				if(item == node) continue;

				item.GetGameObject().transform.localPosition -= o;
			}
		}
		else
		{
			node.GetGameObject().transform.localPosition = node.GetRuntimeGraph().scrollRect.content.InverseTransformPoint(eventData.position) - offset;
		}
	}

	public void OnBeginDrag(PointerEventData eventData) {
		Vector2 pointer = node.GetRuntimeGraph().scrollRect.content.InverseTransformPoint(eventData.position);
		Vector2 pos = node.GetGameObject().transform.localPosition;
		offset = pointer - pos;
	}

	public void OnEndDrag(PointerEventData eventData) {
		Vector3 oldLocalPos = node.GetGameObject().transform.localPosition;
		
		node.GetGameObject().transform.localPosition = node.GetRuntimeGraph().scrollRect.content.InverseTransformPoint(eventData.position) - offset;
		Vector2 pos = node.GetGameObject().transform.localPosition;
		pos.y = -pos.y;
		node.SetPosition(pos);

		if(node.GetRuntimeGraph().selectedNodes.Count > 0 && node.GetRuntimeGraph().selectedNodes.Contains(node)){
			Vector3 o = oldLocalPos  - node.GetGameObject().transform.localPosition;
			
			foreach (var item in node.GetRuntimeGraph().selectedNodes)
			{
				if(item == node) continue;

				item.GetGameObject().transform.localPosition -= o;

				Vector2 p = item.GetGameObject().transform.localPosition;
				p.y = -p.y;
				item.SetPosition(p);
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button != PointerEventData.InputButton.Right)
			return;

		node.GetRuntimeGraph().nodeContextMenu.OpenAt(eventData.position, (IContextItem) node);
	}
}