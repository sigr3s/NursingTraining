using UnityEngine;
using UnityEditor;
using NT.Variables;
using System;
using System.Reflection;
using System.Runtime.Serialization;

[CustomEditor(typeof(SceneVariables))]
public class SceneVariablesEditor : Editor{

    string[] options = {typeof(NTString).ToString(), typeof(NTFloat).ToString(), typeof(NTInt).ToString(), typeof(NTBool).ToString()};
    Type[] optionTypes = {typeof(NTString), typeof(NTFloat), typeof(NTInt), typeof(NTBool)};

    int selectedOption = 0;

    INTVaribale current;

    NTVariableData currentData;

    
    

    public override void OnInspectorGUI()
    {
        int newOption = EditorGUILayout.Popup(selectedOption, options);

        if(newOption != selectedOption || current == null){
            selectedOption = newOption;
            Type t = optionTypes[selectedOption];
            current = (INTVaribale)FormatterServices.GetUninitializedObject(t); //does not call ctor
            currentData = new NTVariableData();
        }

        if(selectedOption == -1){

        }
        else
        {
            SceneVariables sv = target as SceneVariables;

            GUILayout.Space(20);

            Type t = optionTypes[selectedOption];
            
            if(sv.variableRepository.dictionary.ContainsKey(t.ToString())){
                NTVariableDictionary ntd = sv.variableRepository.dictionary[t.ToString()];
                
                for(int i = 0; i < ntd.keys.Count; i++){
                    GUILayout.Label(ntd.keys[i]);
                }
            }
            
            currentData.Name  = EditorGUILayout.TextField("Key", currentData.Name);
            currentData.Value = EditorGUILayout.TextField("Value (Will be deserialized by type)", currentData.Value );

            currentData.DefaultValue = currentData.Value;

            if(GUILayout.Button("Create variable"))
            {
                current.FromNTVariableData(currentData);
                sv.variableRepository.AddVariable(current, t);
                EditorUtility.SetDirty(sv);

                current = null;
            }
        }

        base.OnInspectorGUI();
    }
}