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
        return ToJSON(o, parent,ignoreFields, new List<Type>());
    }

    public static JSONNode ToJSON(object o, JSONNode parent, List<Type> referencedTypes){
        return ToJSON(o, parent,new List<string>(), referencedTypes);
    }

    public static JSONNode ToJSON(object o, JSONNode parent, List<string> ignoreFields, List<Type> referencedTypes){
        return ToJSONInternal(o, o.GetType(), parent,ignoreFields, new List<Type>(), referencedTypes);
    }

    private static JSONNode ToJSONInternal( object o, Type objectType, JSONNode parent,
                                            List<string> ignoreFields, List<Type> visitedTypes,
                                            List<Type> referencedTypes){
        if(visitedTypes.Contains(objectType)){
            return parent;
        }

        visitedTypes.Add(objectType);

        bool asignedReference = false;

        if(TryToAdd("",o, objectType, ref parent, ignoreFields, visitedTypes, referencedTypes)){
            return parent;
        }

        FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach(FieldInfo field in fields){
            Type fieldType = field.FieldType;
            string fieldName = field.Name;

            if(ignoreFields.Contains(fieldName)) continue;

            if(o == null) continue;

            object fieldObject = field.GetValue(o);

            List<Type> visited = new List<Type>(visitedTypes);
            asignedReference = false;

            foreach (Type refType in referencedTypes)
            {

                if(refType.IsAssignableFrom(fieldType)){
                    parent.Add(fieldName, fieldObject.GetHashCode());
                    asignedReference = true;
                    break;
                }
            }

            if(asignedReference){
                continue;
            }

            if(!TryToAdd(fieldName, fieldObject,fieldType, ref parent, ignoreFields, visited, referencedTypes)){

                JSONObject jSONObject = new JSONObject();

                jSONObject = (JSONObject) ToJSONInternal(fieldObject, fieldType, jSONObject, ignoreFields, visited, referencedTypes);

                parent.Add(fieldName,jSONObject);
            }
        }
        return parent;
    }

    private static bool TryToAdd  ( string objectName, object o, Type objectType, ref JSONNode parent,
                                    List<string> ignoreFields, List<Type> visitedTypes,
                                    List<Type> referencedTypes){
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

            bool asignedReference = false;
            
            if(fieldList.Count > 0){
                if(objectName == "callbackNodes") Debug.Log("Serializing??");

                var item = fieldList[0];
                if(referencedTypes != null && item != null){
                    foreach (Type refType in referencedTypes){
                        if(refType.IsAssignableFrom(item.GetType())){
                            asignedReference = true;
                            break;
                        }
                    }
                }
               
            }

            foreach (var objectItem in fieldList)
            {
                if(objectItem == null) continue;

                List<Type> visited = new List<Type>(visitedTypes);

                if(asignedReference){
                    fieldListJSON.Add(objectItem.GetHashCode() );
                }
                else if(TryToAdd("", objectItem, objectItem.GetType(),ref fieldListJSON, ignoreFields,visited, referencedTypes)){

                }
                else{
                    JSONObject listItemJSON = new JSONObject();
                    listItemJSON = (JSONObject) ToJSONInternal(objectItem, objectItem.GetType() ,listItemJSON, ignoreFields, visited, referencedTypes);
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

        FromJSON(ref deserializationObject, typeof(T), node, ignoreFields, new Dictionary<int,object>());

        return (T) deserializationObject;
    }

    public static T FromJSON<T>(JSONNode node){
        object deserializationObject = new object();

        FromJSON(ref deserializationObject, typeof(T), node, new List<string>(), new Dictionary<int, object>());

        return (T) deserializationObject;
    }

    public static void FromJSON(ref object o, Type objectType, JSONNode node, List<string> ignoreFields, Dictionary<int, object> references){
        if(TryToAssign(ref o, objectType, node, references)){
            return;
        }

        FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach(FieldInfo field in fields){
            Type fieldType = field.FieldType;
            string fieldName = field.Name;

            if(ignoreFields.Contains(fieldName)) continue;
            if(!node.HasKey(fieldName)) continue;

            object fieldObject = null;
            try{
                fieldObject = FormatterServices.GetUninitializedObject(fieldType);
            }
            catch (Exception e){
                continue;
            }

            JSONNode fieldNode = node[fieldName];

            if(TryToAssign(ref fieldObject, fieldType, fieldNode, references)){

            }
            else
            {
                FromJSON(ref fieldObject, fieldType, fieldNode, ignoreFields, references);
            }

            field.SetValue(o, fieldObject);
        }
    }

    private static bool TryToAssign(ref object o, Type objectType, JSONNode node, Dictionary<int, object> references){
        if(node.IsNumber){
            int id = node.AsInt;

            if(references.ContainsKey(id)){
                object ro = references[id];
                o = ro;
                return true;
            }
        }


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

                if(TryToAssign(ref arrayObject, arrayType, jarray[i], references)){
                }
                else
                {
                    FromJSON(ref arrayObject, arrayType, jarray[i], new List<string>(), references);
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
                object listObject = null;
                try{
                    listObject = FormatterServices.GetUninitializedObject(listType);
                }
                catch (Exception e){

                }


                if(TryToAssign(ref listObject, listType, item, references)){
                }
                else
                {
                    FromJSON(ref listObject, listType, item, new List<string>(),references);
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