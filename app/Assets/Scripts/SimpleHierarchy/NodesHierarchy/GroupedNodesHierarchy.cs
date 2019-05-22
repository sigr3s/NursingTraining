using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NT.Graph;
public class GroupedNodesHierarchy : GUIHierarchy
{
    public UGUIContextMenu contextMenu;


    private void Start() {
        Rebuild();
        SessionManager.Instance.OnShowingGraphChanged.AddListener(Rebuild);
    }

    private void OnEnable() {
        Rebuild();
    }

    public override List<HierarchyModel> GetRoot(){
        List<HierarchyModel> root = new List<HierarchyModel>();


        List<NodeGroupGraph> userGroups = NodeGroupGraph.GetAll();

        foreach (var groupedNodes in userGroups)
        {
            root.Add(new HierarchyModel(
                    new GroupedNodeHierarchyData{ name = groupedNodes.name, key = groupedNodes.name, nodeGroup = groupedNodes}
                ));
            
        }
        return root;
    }

    public void ShowContextMenu(IContextItem sceneHierarcyItem, Vector2 position)
    {
        contextMenu.OpenAt(position, sceneHierarcyItem);
    }

}

public class GroupedNodeHierarchyData : HierarchyData{
    public NodeGroupGraph nodeGroup;
}