using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NT.Graph;

public class UserVariablesHierarchy : GUIHierarchy
{
    public UGUIContextMenu contextMenu;


    private void Start() {
        Rebuild();
        SessionManager.Instance.OnUserVariablesModified.AddListener(Rebuild);
    }

    private void OnEnable() {
        Rebuild();
    }

    public override List<HierarchyModel> GetRoot(){
        List<HierarchyModel> root = new List<HierarchyModel>();
        return root;
    }

    public void ShowContextMenu(IContextItem sceneHierarcyItem, Vector2 position)
    {
        contextMenu.OpenAt(position, sceneHierarcyItem);
    }



}