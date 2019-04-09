using System;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyModel {
    public HierarchyData data;
    public GameObject uiObject;
    private List<HierarchyModel> children = new List<HierarchyModel>();

    private HierarchyModel(){}

    public HierarchyModel(HierarchyData data){
        this.data = data;
    }

    public void AddChild(HierarchyModel child){
        children.Add(child);
    }

    public List<HierarchyModel> GetChildren()
    {
        return children;
    }
}


[System.Serializable]
public class HierarchyData{
    public string name;
    public bool selected;
    public string key;
}