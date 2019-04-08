using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneObjects", menuName = "app/SceneObjects", order = 0)]
public class SceneObjects : ScriptableObject {
    public List<SceneObject> objectSet;


    public SceneObject GetObject(string guid){
        if(string.IsNullOrEmpty(guid)) return null;

        foreach (var item in objectSet)
        {
            if(guid.Equals(item.GetGUID())) return item;
        }

        return null;
    }

}