using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace NT.Variables
{
    public static class VariableEditorHelper
    {
        public static void DrawObject(string name, ref object myData)
        {
            if(myData == null) return;

            Type objectType = myData.GetType();
            if(TryToDraw(name,ref myData, objectType)){
                return;
            }

            int indentLevel = EditorGUI.indentLevel;

            EditorGUILayout.LabelField(name);

            EditorGUI.indentLevel++;

            FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach(FieldInfo field in fields){
                string fieldName = field.Name;
                object data = field.GetValue(myData);
                Type fieldType = field.FieldType;

                if(TryToDraw(fieldName,ref data, fieldType)){
                    field.SetValue(myData, data);
                }
                else
                {
                    EditorGUILayout.LabelField("Not supported nested levels yet");
                }
            }

            EditorGUI.indentLevel--;

            return;

        }

        private static bool TryToDraw(string name, ref object myData, Type objectType)
        {
            if(objectType == typeof(string)){
                myData = EditorGUILayout.TextField(name, (string) myData);
            }
            else if(objectType == typeof(bool)){
                myData = EditorGUILayout.Toggle(name, (bool) myData);
            }
            else if(objectType == typeof(int)){
                myData = EditorGUILayout.IntField(name, (int) myData);
            }
            else if(objectType == typeof(double) ){
                myData = EditorGUILayout.DoubleField(name, (double) myData);
            }
            else if(objectType == typeof(float) ){
                myData = EditorGUILayout.FloatField(name, (float) myData);
            }
            else if(objectType.IsArray){
                Array array = (Array) myData;
                Type arrayType = objectType.GetElementType();

                EditorGUILayout.LabelField("Array not supported yet");

                for(int i = 0; i < array.Length; i++){


                }

            }
            else if(typeof(IList).IsAssignableFrom(objectType)){
                IList list = (IList) myData;

                EditorGUILayout.LabelField("List not supported yet");

                if(list == null){
                    return true;
                }

                foreach (var item in list)
                {

                }
            }
            else{
                return false;
            }

        return true;
        }
    }
}