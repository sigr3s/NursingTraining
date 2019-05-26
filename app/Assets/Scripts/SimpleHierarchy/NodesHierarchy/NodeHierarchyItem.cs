using NT;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

public class NodeHierarchyItem : DraggableGUIHierarchyItem, IBeginDragHandler, IDragHandler, IEndDragHandler {
  
 
    public override void OnDragEnd(PointerEventData eventData)
    {
        NodeHierarchyData nhd = (NodeHierarchyData) data;

        if(nhd.nodeType == null) return;
        
        Node node = rg.graph.AddNode(nhd.nodeType);
        node.position = nodePosition;

        if(node is NTNode){
            node.name = ((NTNode) node).GetDisplayName();
        }

        if(nhd.onNodeCreated != null){
            nhd.onNodeCreated.Invoke(node);
        }
        
        rg.Refresh();
    }
    
}