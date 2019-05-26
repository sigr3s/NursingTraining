using System;
using NT.Nodes.Variables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class UserVariablesHierarchyItem : DraggableGUIHierarchyItem, IPointerClickHandler, IContextItem {
   
    public GetSetContextMenu contextMenu;

    public override void OnDataSetted(){
               
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
            GetComponentInParent<UserVariablesHierarchy>().ShowContextMenu(this, eventData.position);
        }
    }


   
    public override void OnDragEnd(PointerEventData eventData){
        contextMenu.OpenAt(eventData.position, null);
        contextMenu.SetAction = CreateSetNode;
        contextMenu.GetAction = CreateGetNode;
       
    }

    private void CreateGetNode()
    {
        throw new NotImplementedException();
    }

    private void CreateSetNode()
    {
        throw new NotImplementedException();
    }

    public void Remove()
    {
        throw new System.NotImplementedException();
    }

    public string GetKey()
    {
        throw new System.NotImplementedException();
    }
}