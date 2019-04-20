using System;
using NT.SceneObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneHierarcyItem : GUIHierarchyItem, IPointerClickHandler {
    public Button graphButton;
    public override void OnDataSetted(){
        SceneGameObject sceneGameObject = SessionManager.Instance.GetSceneGameObject(data.name);

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
            PrefabObject.CreatePrefab(data.name, SessionManager.Instance.GetSceneGameObject(data.key));           
        }
    }

    private void Start() {
        graphButton.onClick.AddListener(OpenGraph);
    }

    private void OpenGraph()
    {
       SessionManager.Instance.OpenGraphFor(data.key);
    }
}