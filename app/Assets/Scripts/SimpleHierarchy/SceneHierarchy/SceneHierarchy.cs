using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;

public class SceneHierarchy : GUIHierarchy {

    private void Start() {
        SessionManager.Instance.sceneVariables.variableRepository.onModified.AddListener(Rebuild);
        SessionManager.Instance.OnSessionLoaded.AddListener(Rehook);
    }

    private void Rehook(){
        SessionManager.Instance.sceneVariables.variableRepository.onModified.RemoveListener(Rebuild);
        SessionManager.Instance.sceneVariables.variableRepository.onModified.AddListener(Rebuild);

        Rebuild();
    }

    public override List<HierarchyModel> GetRoot(){
        List<HierarchyModel> root = new List<HierarchyModel>();

        NTVariableRepository repo = SessionManager.Instance.sceneVariables.variableRepository;

        foreach (var kvp in repo.dictionary)
        {
            string variable = kvp.Key;
            string displayName = variable.Replace("NT.Variables.NT", "");
            NTVariableDictionary varDict = kvp.Value;

            if(!typeof(INTSceneObject).IsAssignableFrom(varDict._dictType)){
                continue;
            }

            foreach (var kvpi in varDict)
            {
                HierarchyModel model = new HierarchyModel(new HierarchyData{ name = kvpi.Key });
                   
                root.Add(model);


            }
        }

        return root;
    }
}