using System;
using NT.Nodes.Variables;
using NT.SceneObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class SceneHierarcyItem : DraggableGUIHierarchyItem, IPointerClickHandler, IContextItem {
    public Button graphButton;

    public override void OnDataSetted(){
        SceneGameObject sceneGameObject = SessionManager.Instance.GetSceneGameObject(data.key);

        if(sceneGameObject != null){
            if(sceneGameObject.sceneObject.GetCallbacks().Count > 0){
                graphButton.gameObject.SetActive(true);
            }
            else
            {
                graphButton.gameObject.SetActive(false);                
            }
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left){
            if(eventData.clickCount > 1){        
                SessionManager.Instance.SetSelected(data.key);
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            GetComponentInParent<SceneHierarchy>().ShowContextMenu(this, eventData.position);
        }
    }

    private void Start() {
        graphButton.onClick.AddListener(OpenGraph);
    }

    private void OpenGraph()
    {
       SessionManager.Instance.OpenGraphFor(data.key);
    }

    public void Remove()
    {
        SessionManager.Instance.RemoveSceneGameObject(data.key);
    }

    public string GetKey()
    {
        return data.key;
    }

    public override void OnDragEnd(PointerEventData eventData)
    {        
        Node node = rg.graph.AddNode(typeof(GetNTVariableNode));
        node.position = nodePosition;
        node.name = "GET - (" + data.name + ")";

        ((GetNTVariableNode) node).SetVariableKey(data.key, typeof(string), "", typeof(SceneGameObject) );
       
        rg.Refresh();
    }

}