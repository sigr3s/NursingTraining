using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using SimpleJSON;
using UnityEngine;

public static class SimpleJSONExtension {
    public static JSONNode ToJSON(object o){
        JSONNode parent = new JSONObject();
        return ToJSON(o, parent, new List<string>());
    }

    public static JSONNode ToJSON(object o, JSONNode parent, List<string> ignoreFields){
        return ToJSONInternal(o, o.GetType(), parent,ignoreFields, new List<Type>());
    }

    private static JSONNode ToJSONInternal(object o, Type objectType, JSONNode parent, List<string> ignoreFields, List<Type> visitedTypes){
        if(visitedTypes.Contains(objectType)){
            return parent;
        }

        if(TryToAdd("",o, objectType, ref parent, visitedTypes)){
            return parent;
        }

        FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach(FieldInfo field in fields){
            Type fieldType = field.FieldType;
            string fieldName = field.Name;

            if(ignoreFields.Contains(fieldName)) continue;

            object fieldObject = field.GetValue(o);

            List<Type> visited = new List<Type>(visitedTypes);

            if(!TryToAdd(fieldName, fieldObject,fieldType, ref parent, visited)){
                JSONObject jSONObject = new JSONObject();

                jSONObject = (JSONObject) ToJSONInternal(fieldObject, fieldType, jSONObject, ignoreFields, visited);

                parent.Add(fieldName,jSONObject);
            }
        }
        return parent;
    }

    private static bool TryToAdd(string objectName, object o, Type objectType, ref JSONNode parent, List<Type> visitedTypes){
        if(objectType == typeof(string))
        {
            if(!string.IsNullOrEmpty(objectName)) parent.Add(objectName, (string) o);
            else parent.Add((string) o);
        }
        else if(objectType.IsArray || typeof(IList).IsAssignableFrom(objectType) )
        {
            System.Collections.IList fieldList = (System.Collections.IList) o;
            if(fieldList == null) return parent;

            JSONNode fieldListJSON = new JSONArray();

            foreach (var objectItem in fieldList)
            {
                if(objectItem == null) continue;

                List<Type> visited = new List<Type>(visitedTypes);

                if(TryToAdd("", objectItem, objectItem.GetType(),ref fieldListJSON, visited)){

                }
                else{
                    JSONObject listItemJSON = new JSONObject();
                    listItemJSON = (JSONObject) ToJSONInternal(objectItem, objectItem.GetType() ,listItemJSON, new List<string>(), visited);
                    fieldListJSON.Add(listItemJSON);
                }

            }

            if(!string.IsNullOrEmpty(objectName)) parent.Add(objectName, fieldListJSON);
            else parent.Add(fieldListJSON);
        }
        else if(objectType == typeof(float) )
        {
            if(!string.IsNullOrEmpty(objectName)) parent.Add(objectName,new JSONNumber((float) (o)));
            else parent.Add(new JSONNumber((float) (o)));
        }
        else if(objectType == typeof(int))
        {
            if(!string.IsNullOrEmpty(objectName)) parent.Add(objectName,new JSONNumber((int) (o)));
            else parent.Add(new JSONNumber((int) (o)));
        }
        else if(objectType == typeof(double)){
            if(!string.IsNullOrEmpty(objectName)) parent.Add(objectName,new JSONNumber((double) (o)));
            else parent.Add(new JSONNumber((double) (o)));
        }
        else if(objectType == typeof(bool))
        {
            if(!string.IsNullOrEmpty(objectName)) parent.Add(objectName,new JSONBool((bool) (o)));
            else parent.Add(new JSONBool((bool) (o)));
        }
        else
        {
            return false;
        }

        return true;
    }

    public static T FromJSON<T>(JSONNode node, List<string> ignoreFields){
        object deserializationObject = new object();

        FromJSON(ref deserializationObject, typeof(T), node, ignoreFields);

        return (T) deserializationObject;
    }
    
    public static void FromJSON(ref object o, Type objectType, JSONNode node, List<string> ignoreFields){
        if(TryToAssign(ref o, objectType, node)){
            return;
        }

        FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach(FieldInfo field in fields){
            Type fieldType = field.FieldType;
            string fieldName = field.Name;

            if(ignoreFields.Contains(fieldName)) continue;
            if(!node.HasKey(fieldName)) continue;

            object fieldObject = FormatterServices.GetUninitializedObject(fieldType);

            JSONNode fieldNode = node[fieldName];

            if(TryToAssign(ref fieldObject, fieldType, fieldNode)){

            }
            else
            {
                FromJSON(ref fieldObject, fieldType, fieldNode, ignoreFields);
            }

            field.SetValue(o, fieldObject);
        }
    }

    private static bool TryToAssign(ref object o, Type objectType, JSONNode node){
        if(objectType == typeof(string)){
            o = (string) node;
        }
        else if(objectType.IsArray){
            JSONArray jarray =  node.AsArray;
            int leng = jarray.Count;

            Array array = (Array) Activator.CreateInstance(objectType, new object[] { leng }); // Length 1
            Type arrayType = objectType.GetElementType();

            for(int i = 0; i < leng; i++){
                object arrayObject = FormatterServices.GetUninitializedObject(arrayType);

                if(TryToAssign(ref arrayObject, arrayType, jarray[i])){
                }
                else
                {
                    FromJSON(ref arrayObject, arrayType, jarray[i], new List<string>());
                }
                array.SetValue(arrayObject, i);
            }

            o = array;

        }
        else if(typeof(IList).IsAssignableFrom(objectType)){
            IList list = (IList) Activator.CreateInstance(objectType);

            Type listType = objectType.GetGenericArguments().Single();
            JSONArray array =  node.AsArray;

            foreach (var item in array.Values)
            {
                object listObject = FormatterServices.GetUninitializedObject(listType);


                if(TryToAssign(ref listObject, listType, item)){
                }
                else
                {
                    FromJSON(ref listObject, listType, item, new List<string>());
                }

                list.Add(listObject);
            }


            o = list;
        }
        else if(objectType == typeof(int)){
            o = (int) node.AsInt;
        }
        else if(objectType == typeof(float)){
            o = (float) node.AsFloat;
        }
        else if(objectType == typeof(double)){
            o = (double) node.AsDouble;
        }
        else if(objectType == typeof(bool)){
            o = (bool) node.AsBool;
        }
        else{
            return false;
        }

        return true;
    }
}