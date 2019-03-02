using XNode;
using NT.Atributes;
using XNodeEditor;
using UnityEngine;
using NT.Variables;
using UnityEditor;
using NT.Graph;
using System.Linq;
using System;
using System.Reflection;

namespace NT.Nodes.Variables
{
    [CustomNodeEditor(typeof(GetNTVariableNode))]
    public class NTVariableNodeEditor : NodeEditor {

        public override void OnBodyGUI() {
            NTNode node = target as NTNode;
            IVariableNode ivn = target as IVariableNode;
            NTGraph graph =  node.graph as NTGraph;

            int _choiceIndex = 0;
            string[] _choices = graph.sceneVariables.variableRepository.GetOptions(ivn.GetVariableType() , ivn.GetVariableKey(), out _choiceIndex).ToArray();
            int _newChoiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);

            if(_newChoiceIndex != _choiceIndex){
                ivn.SetVariableKey(_choices[_newChoiceIndex]);
            }

            serializedObject.Update();

            string[] excludes = { "m_Script", "graph", "position", "ports" };
            SerializedProperty iterator = serializedObject.GetIterator();

            bool enterChildren = true;
            EditorGUIUtility.labelWidth = 84;
            while (iterator.NextVisible(enterChildren)) {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                //EditorGUILayout.PropertyField(iterator, GUILayout.MinWidth(30));
            }
            serializedObject.ApplyModifiedProperties();

            base.OnBodyGUI();

            ExtraBody();
        }


        public virtual void ExtraBody(){

        }

        protected void DrawObject(string name, ref object myData)
        {
            Type objectType = myData.GetType();
            if(TryToDraw(name,ref myData, objectType)){
                return;
            }

            FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach(FieldInfo field in fields){
            }

            return;

        }

        private bool TryToDraw(string name, ref object myData, Type objectType)
        {
            if(objectType == typeof(string)){
                myData = EditorGUILayout.TextField(name, (string) myData);
            }
            if(objectType == typeof(bool)){
                myData = EditorGUILayout.Toggle(name, (bool) myData);
            }
            else{
                return false;
            }

            return true;
        }

        public override Color GetTint(){
            NTNode node = target as NTNode;
            IVariableNode ivn = target as IVariableNode;

            if(node.isExecuting){
                return Color.red;
            }
            else
            {
                NTGraph graph =  node.graph as NTGraph;
                Type t = ivn.GetVariableType();

                if(t != null){
                    return graph.sceneVariables.GetColorFor(t);
                }
                return base.GetTint();
            }
        }
    }


    [CustomNodeEditor(typeof(SetNTVariableNode))]
    public class SetNTVariableNodeEditor : NTVariableNodeEditor{
        public override void ExtraBody(){
            SetNTVariableNode sntv = (SetNTVariableNode) target;
            if(sntv != null){
                if(sntv._myData != null && !sntv.GetPort(sntv.variableField).IsConnected){
                    sntv._myData.FromNTVariableData(sntv.data);

                    EditorGUI.BeginChangeCheck();
                    object value = sntv._myData.GetValue();

                    if(value == null) return;

                    DrawObject("value", ref value);

                    if(EditorGUI.EndChangeCheck()){
                        sntv._myData.SetKey(sntv.variableKey);
                        sntv._myData.SetValue(value);
                        sntv.data = sntv._myData.ToNTVariableData();
                    }
                }
            }
        }
    }
}