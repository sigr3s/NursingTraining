using System;
using NT;
using NT.SceneObjects;
using NT.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeInspector : GUIInspector {


    [Header("References")]    
    public TextMeshProUGUI title;
    public TextMeshProUGUI over;
    public GetSetContextMenu contextMenu;

    [Header("Debug")]
    public SceneGameObject current;

    private void Start() {
        SessionManager.Instance.OnCurrentChanged.AddListener(OnCurrentChanged);
    }

    private void OnCurrentChanged()
    {
        current = SessionManager.Instance.selectedSceneObject;
        object value = null;

        if(current != null){
            value = current.data.data.GetDefaultValue();
            SetCurrent(current.sceneObject.GetDisplayName(), value);
        }
        else
        {
            Inspect(null, null);
        }

    }

    public void SetCurrent(string name, object Value, GameObject currentGo = null){
        title.text = name;

        Inspect(Value, OnChanged);

        var draggables = GetComponentsInChildren<DraggableGUIProperty>();

        foreach(var draggable in draggables){
            draggable.contextMenu = this.contextMenu;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    private void OnChanged(object value)
    {
        current.data.data.SetDefaultValue(value);
    }
}