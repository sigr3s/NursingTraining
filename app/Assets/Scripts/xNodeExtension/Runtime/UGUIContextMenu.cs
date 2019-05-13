using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

public class UGUIContextMenu : MonoBehaviour, IPointerExitHandler {

	public CanvasGroup group;
	protected IContextItem selected;

	private void Start() {
		Close();
		Initialize();
	}

	public virtual void Initialize(){

	}

	public void OpenAt(Vector2 pos, IContextItem context) {
		transform.position = pos;
		group.alpha = 1;
		group.interactable = true;
		group.blocksRaycasts = true;
		transform.SetAsLastSibling();

		selected = context;
	}

	public void Close() {
		group.alpha = 0;
		group.interactable = false;
		group.blocksRaycasts = false;
	}


	public void OnPointerExit(PointerEventData eventData) {
		Close();
	}
}