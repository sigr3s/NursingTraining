using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "SceneObjects", menuName = "app/SceneObjects", order = 0)]
public class SceneObjects : ScriptableObject {
    public List<SceneObject> objectSet;
    public List<PrefabObject> prefabSet;

    public List<Sprite> prefabSprites;


    public void LoadPrefabs() {

        prefabSet = new List<PrefabObject>();
        DirectoryInfo prefabsDir = new DirectoryInfo(PrefabObject.exportPath);

        if(!prefabsDir.Exists) return;

        FileInfo[] files = prefabsDir.GetFiles("*.nt");

        foreach(var file in files){
            PrefabObject po = PrefabObject.LoadPrefab(file.FullName);

            if(po == null){
                Debug.LogWarning("Cannot load???");
            }
            
            prefabSet.Add(po);
        }

    }

    public SceneObject GetObject(string guid){
        if(string.IsNullOrEmpty(guid)) return null;

        foreach (var item in objectSet)
        {
            if(guid.Equals(item.GetGUID())) return item;
        }

        foreach (var prefab in prefabSet)
        {
            if(guid.Equals(prefab.GetGUID())) return prefab;
        }

        return null;
    }

}