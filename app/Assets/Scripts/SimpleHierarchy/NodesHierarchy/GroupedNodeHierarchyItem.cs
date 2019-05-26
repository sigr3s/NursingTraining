using NT.Graph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GroupedNodeHierarchyItem : DraggableGUIHierarchyItem, IPointerClickHandler, IContextItem {

    public override void OnDragEnd(PointerEventData eventData)
    {
        GroupedNodeHierarchyData nhd = (GroupedNodeHierarchyData) data;

        if(nhd.nodeGroup == null) return; 
        
        rg.AddNodeGroup(nhd.nodeGroup , nodePosition);
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            GetComponentInParent<GroupedNodesHierarchy>().ShowContextMenu(this, eventData.position);
        }
    }

    public void Remove()
    { 
        GroupedNodeHierarchyData nhd = (GroupedNodeHierarchyData) data;
        NodeGroupGraph.Remove(nhd.nodeGroup.assetID);
        GetComponentInParent<GroupedNodesHierarchy>().Rebuild();
    }

    public string GetKey()
    {
        GroupedNodeHierarchyData nhd = (GroupedNodeHierarchyData) data;
        return nhd.nodeGroup.assetID;
    }
}