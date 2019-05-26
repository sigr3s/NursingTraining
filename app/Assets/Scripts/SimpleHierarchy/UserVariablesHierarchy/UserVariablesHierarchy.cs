using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NT.Graph;
using TMPro;

public class UserVariablesHierarchy : GUIHierarchy
{
    public GetSetContextMenu GetSetContextMenu;
    public UGUIContextMenu RemoveContextMenu;


    public TMP_InputField variableName;


    private void Start() {
        Rebuild();
        SessionManager.Instance.OnUserVariablesModified.AddListener(Rebuild);
    }

    private void OnEnable() {
        Rebuild();
    }

    public override List<HierarchyModel> GetRoot(){
        List<HierarchyModel> root = new List<HierarchyModel>();



        foreach (var item in SessionManager.Instance.userVariables)
        {
            root.Add(new HierarchyModel(new UserVariableHierarchyData{name = item.Key, key = item.Key, 
            contextMenu = GetSetContextMenu, data = item.Value.defaultValue }));
        }
        return root;
    }

    public void ShowContextMenu(IContextItem sceneHierarcyItem, Vector2 position)
    {
        RemoveContextMenu.OpenAt(position, sceneHierarcyItem);
    }
    

    public void CreateBool(){
        if(!string.IsNullOrEmpty(variableName.text)){
            string name = variableName.text;
            SessionManager.Instance.SetDefaultUserVariable(name, false);
            variableName.text = "";
        }
    }

    
    public void CreateNumber(){
        if(!string.IsNullOrEmpty(variableName.text)){
            string name = variableName.text;
            SessionManager.Instance.SetDefaultUserVariable(name, 0.0f);
            variableName.text = "";
        }       
    }

    
    public void CreateString(){
        if(!string.IsNullOrEmpty(variableName.text)){
            string name = variableName.text;
            SessionManager.Instance.SetDefaultUserVariable(name, "");
            variableName.text = "";
        }
    }


}

public class UserVariableHierarchyData : HierarchyData{
    public GetSetContextMenu contextMenu;
    public object data;
}