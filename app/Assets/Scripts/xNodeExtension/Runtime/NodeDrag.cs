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
		node.GetGameObject().transform.localPosition = node.GetRuntimeGraph().scrollRect.content.InverseTransformPoint(eventData.position) - offset;
	}

	public void OnBeginDrag(PointerEventData eventData) {
		Vector2 pointer = node.GetRuntimeGraph().scrollRect.content.InverseTransformPoint(eventData.position);
		Vector2 pos = node.GetGameObject().transform.localPosition;
		offset = pointer - pos;
	}

	public void OnEndDrag(PointerEventData eventData) {
		node.GetGameObject().transform.localPosition = node.GetRuntimeGraph().scrollRect.content.InverseTransformPoint(eventData.position) - offset;
		Vector2 pos = node.GetGameObject().transform.localPosition;
		pos.y = -pos.y;
		node.SetPosition(pos);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button != PointerEventData.InputButton.Right)
			return;

		node.GetRuntimeGraph().nodeContextMenu.OpenAt(eventData.position, (IContextItem) node);
	}
}