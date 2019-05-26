using System;
using NT.Nodes.Variables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class UserVariablesHierarchyItem : DraggableGUIHierarchyItem, IPointerClickHandler, IContextItem {

    [Header("Prefab references")]
    public GUIProperty property;

    [Header("Debug")]
    public GetSetContextMenu contextMenu;

    public override void OnDataSetted(){
        UserVariableHierarchyData uhd = (UserVariableHierarchyData) data;
        if(data != null){
            contextMenu = uhd.contextMenu;

            if(uhd.data.GetType().IsNumber()){
                property.SetData(uhd.data, data.name,GUIProperty.PropertyType.Number);
            }
            else if(uhd.data.GetType().IsBool()){
                property.SetData(uhd.data, data.name,GUIProperty.PropertyType.Boolean);
            }
            else
            {
                property.SetData(uhd.data, data.name,GUIProperty.PropertyType.String);
            }

            property.OnValueChanged.RemoveAllListeners();
            property.OnValueChanged.AddListener(SetUserVariable);
        }
    }

    private void SetUserVariable(object value, string arg1)
    {
        SessionManager.Instance.SetDefaultUserVariable(data.key, value);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
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
        UserVariableHierarchyData uhd = (UserVariableHierarchyData) data;

        Node node = rg.graph.AddNode(typeof(GetUserVariableNode));
        node.position = nodePosition;
        node.name = "GET - (" + data.key + ")";

        ((GetUserVariableNode) node).SetVariableKey(data.key, uhd.data.GetType() );

        rg.Refresh();
    }

    private void CreateSetNode()
    {
        UserVariableHierarchyData uhd = (UserVariableHierarchyData) data;

        Node node = rg.graph.AddNode(typeof(SetUserVariableNode));
        node.position = nodePosition;
        node.name = "SET - (" + data.key + ")";

        ((SetUserVariableNode) node).SetVariableKey(data.key, uhd.data.GetType() );

        rg.Refresh();
    }

    public void Remove()
    {
        SessionManager.Instance.RemoveUserVariable(data.key);
    }

    public string GetKey()
    {
        return data.key;
    }
}