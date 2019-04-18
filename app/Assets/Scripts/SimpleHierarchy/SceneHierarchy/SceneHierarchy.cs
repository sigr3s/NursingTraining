using System;
using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;

public class SceneHierarchy : GUIHierarchy {

    private void Start() {
        SessionManager.Instance.OnSceneGameObjectsChanged.AddListener(Rebuild);
        SessionManager.Instance.OnSessionLoaded.AddListener(Rehook);
        SessionManager.Instance.OnCurrentChanged.AddListener(Rebuild);

        Rebuild();
    }


    private void Rehook(){

        SessionManager.Instance.sceneVariables.variableRepository.onModified.RemoveListener(Rebuild);
        SessionManager.Instance.sceneVariables.variableRepository.onModified.AddListener(Rebuild);

        Rebuild();
    }

    public override List<HierarchyModel> GetRoot(){

        Debug.Log("Get rooot!");

        List<HierarchyModel> root = new List<HierarchyModel>();

        var parents = new Dictionary<string, HierarchyModel>();
        var childs = new Dictionary<string, List<HierarchyModel>>();

        foreach(var sgoKVp in SessionManager.Instance.sceneGameObjects){
            SceneGameObject scgo = sgoKVp.Value;

            bool selected = SessionManager.Instance.selectedSceneObject?.NTKey == sgoKVp.Key;
            HierarchyModel model = new HierarchyModel(new HierarchyData{ name = sgoKVp.Value.name, key = sgoKVp.Key, selected = selected});

            if(childs.ContainsKey(sgoKVp.Key)){
                List<HierarchyModel> scgoChilds = childs[sgoKVp.Key];

                foreach(HierarchyModel hm in scgoChilds){
                    model.AddChild(hm);
                }

                childs.Remove(sgoKVp.Key);
            }

            if(scgo.parent != null){
                string parentKey = scgo.parent.NTKey;

                if(parents.ContainsKey(parentKey)){
                    parents[scgo.parent.NTKey].AddChild(model);
                }
                else
                {
                    if(childs.ContainsKey(parentKey)){
                        List<HierarchyModel> scgoChilds = childs[parentKey];
                        scgoChilds.Add(model);
                        childs[parentKey] = scgoChilds;
                    }
                    else{
                        List<HierarchyModel> scgoChilds = new List<HierarchyModel>();
                        scgoChilds.Add(model);
                        childs.Add(parentKey, scgoChilds);
                    }
                }
            }
            else
            {
                root.Add(model);
            }

            parents.Add(sgoKVp.Key, model);

        }

        return root;
    }
}