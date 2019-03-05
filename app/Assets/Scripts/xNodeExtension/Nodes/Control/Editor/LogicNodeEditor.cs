using UnityEngine;
using UnityEditor;
using XNodeEditor;
using NT.Variables;
using NT.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NT.Nodes.Control
{
    [CustomNodeEditor(typeof(LogicNode))]
    public class LogicNodeEditor : NodeEditor {
        string[] options;
        Type[] optionTypes;
        List<Type> optionsT = new List<Type>();
        List<string> optionsL = new List<string>();
        List<Type> optionsTL = new List<Type>();

        public override void OnBodyGUI() {
            base.OnBodyGUI();

            if(optionsT.Count == 0){
                optionsT = ReflectionUtilities.variableNodeTypes.ToList();
                optionsL = new List<string>();
                optionsTL = new List<Type>();

                foreach (var opt in optionsT)
                {
                    if(opt.IsGenericTypeDefinition) continue;

                    optionsL.Add(opt.ToString());
                    optionsTL.Add(opt);
                }

                options = optionsL.ToArray();
                optionTypes = optionsTL.ToArray();

            }

            EditorGUILayout.Space();
            LogicNode node = (LogicNode) target;
            NTGraph g = (NTGraph) node.graph;

            if(node.conditions == null){
                node.conditions = new List<LogicCondition>();
            }

            for(int i = 0; i < node.conditions.Count; i++){
                LogicCondition lc = node.conditions[i];

                EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Condition " + i);

                        if(GUILayout.Button("Rm")){
                            node.conditions.RemoveAt(i);
                            i--;
                        }
                    EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Variable Left");

                        lc.leftSide.isVariable = EditorGUILayout.Toggle(lc.leftSide.isVariable);

                    EditorGUILayout.EndHorizontal();

                    if(!lc.leftSide.isVariable){
                        lc.leftSide.value = EditorGUILayout.TextField("V:", lc.leftSide.value);
                    }
                    else
                    {
                        int option = optionsTL.IndexOf(lc.leftSide.VariableType);
                        int newOption = EditorGUILayout.Popup(option, options);

                        if(option != newOption){
                            lc.leftSide.variableType = optionTypes[newOption].AssemblyQualifiedName;
                        }

                        if(!string.IsNullOrEmpty(lc.leftSide.variableType) ){
                            int vInd = 0;
                            List<string> variableOP = g.sceneVariables.variableRepository.GetOptions(lc.leftSide.VariableType, lc.leftSide.variableName, out vInd);

                            int newVInd = EditorGUILayout.Popup(vInd, variableOP.ToArray());

                            if(vInd != newVInd && newVInd >= 0){
                                lc.leftSide.variableName = variableOP[newVInd];
                            }
                        }
                    }


                    lc.op = (Operator) EditorGUILayout.EnumPopup(lc.op);

                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Variable Right");

                        lc.rightSide.isVariable = EditorGUILayout.Toggle(lc.rightSide.isVariable);

                    EditorGUILayout.EndHorizontal();

                    if(!lc.rightSide.isVariable){
                        lc.rightSide.value = EditorGUILayout.TextField("V:", lc.rightSide.value);
                    }
                    else
                    {
                        int option = optionsTL.IndexOf(lc.rightSide.VariableType);
                        int newOption = EditorGUILayout.Popup(option, options);

                        if(option != newOption){
                            lc.rightSide.variableType = optionTypes[newOption].AssemblyQualifiedName;
                        }

                        if(!string.IsNullOrEmpty(lc.rightSide.variableType) ){
                            int vInd = 0;
                            List<string> variableOP = g.sceneVariables.variableRepository.GetOptions(lc.rightSide.VariableType, lc.rightSide.variableName, out vInd);

                            int newVInd = EditorGUILayout.Popup(vInd, variableOP.ToArray());

                            if(vInd != newVInd && newVInd >= 0){
                                lc.rightSide.variableName = variableOP[newVInd];
                            }
                        }
                    }


                EditorGUILayout.Space();

            }


            if(GUILayout.Button("Add")){
                node.conditions.Add(new LogicCondition());
            }

        }
    }

}