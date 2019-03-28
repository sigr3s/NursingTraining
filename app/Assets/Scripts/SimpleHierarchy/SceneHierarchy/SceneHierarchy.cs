using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;

public class SceneHierarchy : GUIHierarchy {
    public SceneVariables variables;

    private void Start() {
        variables =   SceneManager.Instance.sceneVariables;
        variables.variableRepository.onModified.AddListener(Rebuild);
    }

    public override List<HierarchyModel> GetRoot(){
        List<HierarchyModel> root = new List<HierarchyModel>();

        NTVariableRepository repo = variables.variableRepository;

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

                var d = ReflectionUtilities.DesgloseInBasicTypes(kvpi.Value.GetDataType() );

                foreach (var dv in d)
                {
                    foreach(string var in dv.Value){
                        model.AddChild(new HierarchyModel(new HierarchyData{name = var}));
                    }
                }
                   
                root.Add(model);


            }
        }

        return root;
    }
}