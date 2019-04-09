using UnityEngine;
using UnityEngine.EventSystems;

public class SceneHierarcyItem : GUIHierarchyItem, IPointerClickHandler {
    public override void OnDataSetted(){

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount > 1){
            SessionManager.Instance.SetSelected(data.name);
        }
    }
}