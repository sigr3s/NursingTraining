using UnityEngine;
using UnityEditor;
using NT.Variables;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using NT;
using System.Linq;

[CustomEditor(typeof(SceneVariables))]
public class SceneVariablesEditor : Editor{

    string[] options; // {typeof(NTString).ToString(), typeof(NTFloat).ToString(), typeof(NTInt).ToString(), typeof(NTBool).ToString()};
    Type[] optionTypes; //= {typeof(NTString), typeof(NTFloat), typeof(NTInt), typeof(NTBool)};

    int selectedOption = 0;

    NTVariable current;

    List<Type> optionsT = new List<Type>();

    public override void OnInspectorGUI()
    {
        if(optionsT.Count == 0){
            optionsT = ReflectionUtilities.variableNodeTypes.ToList();
            List<string> optionsL = new List<string>();
            List<Type> optionsTL = new List<Type>();

            foreach (var opt in optionsT)
            {
                if(opt.IsGenericTypeDefinition) continue;

                optionsL.Add(opt.ToString());
                optionsTL.Add(opt);
            }

            options = optionsL.ToArray();
            optionTypes = optionsTL.ToArray();

        }
        int newOption = EditorGUILayout.Popup(selectedOption, options);

        if(newOption != selectedOption || current == null){
            selectedOption = newOption;
            Type t = optionTypes[selectedOption];
            Debug.Log("¿??");
            current = (NTVariable)Activator.CreateInstance(t);
            var go = new GameObject();
            go.AddComponent(t);
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
                    NTVariable var;
                    string key = ntd.keys[i];

                    if(ntd.TryGetValue(key, out var)){
                        var.SetCaollapsed(!EditorGUILayout.Foldout(!var.IsCollapsed(), key));

                        if(!var.IsCollapsed()){
                            Debug.Log(var);

                            EditorGUI.BeginChangeCheck();
                                object intval = var.GetValue();
                                VariableEditorHelper.DrawObject("VALUE: ", ref intval);
                            if(EditorGUI.EndChangeCheck()){
                                var.SetValue(intval);
                                var.SetDefaultValue(intval);

                                ntd[key] = var;
                            }

                            if(GUILayout.Button("Remove")){

                            }
                        }
                    }

                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            current.SetKey(EditorGUILayout.TextField("Key", current.GetKey()));

            EditorGUI.BeginChangeCheck();

            object value = current.GetValue();

            VariableEditorHelper.DrawObject("VALUE: ", ref value);

            if(EditorGUI.EndChangeCheck()){

                Debug.Log(current);

                current.SetValue(value);
                current.SetDefaultValue(value);
            }


            if(GUILayout.Button("Create variable"))
            {
                sv.variableRepository.AddVariable(current, t);
                EditorUtility.SetDirty(sv);

                current = null;
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        base.OnInspectorGUI();
    }

}