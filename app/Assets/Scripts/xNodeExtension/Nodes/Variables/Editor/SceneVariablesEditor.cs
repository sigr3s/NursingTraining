using UnityEngine;
using UnityEditor;
using NT.Variables;
using System;
using System.Reflection;

[CustomEditor(typeof(SceneVariables))]
public class SceneVariablesEditor : Editor{

    string[] options = {typeof(NTString).ToString(), typeof(NTFloat).ToString(), typeof(NTInt).ToString(), typeof(NTBool).ToString()};
    Type[] optionTypes = {typeof(NTString), typeof(NTFloat), typeof(NTInt), typeof(NTBool)};

    int selectedOption = -1;

    NTVariable current;

    

    public override void OnInspectorGUI()
    {
        int newOption = EditorGUILayout.Popup(selectedOption, options);

        if(newOption != selectedOption){
            selectedOption = newOption;
            Type t = optionTypes[selectedOption];
            current = (NTVariable) Activator.CreateInstance(t);
        }

        if(selectedOption == -1){

        }
        else
        {
            SceneVariables sv = target as SceneVariables;

            GUILayout.Space(20);

            Type t = optionTypes[selectedOption];
            
            if(sv.variableRepository.dictionary.ContainsKey(t)){
                NTVariableDictionary ntd = sv.variableRepository.dictionary[t];
                
                for(int i = 0; i < ntd.keys.Count; i++){
                    GUILayout.Label(ntd.keys[i]);
                }
            }

            current.SetKey(EditorGUILayout.TextField("key", (string) current.GetKey()) );

            current.serializedValue = EditorGUILayout.TextField("Value", (string) current.serializedValue);

            //EditorGUILayout.(current.GetValue(), current.GetValue().GetType());

            if(GUILayout.Button("Create variable"))
            {
                current.serializedDefaultValue = current.serializedValue;
                current.Deserialize();

                sv.variableRepository.AddVariable(current, t);
                EditorUtility.SetDirty(sv);

            }
        }

        base.OnInspectorGUI();
    }
}