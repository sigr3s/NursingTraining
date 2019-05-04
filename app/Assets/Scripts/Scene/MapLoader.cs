using System.Collections.Generic;
using NT.Graph;
using NT.SceneObjects;
using UnityEngine;

public class MapLoader : MonoBehaviour, IMapLoader {
    public Transform mapPivot;
    public GameObject items;


    private void Awake() {
        SessionManager.Instance.mapLoader = this;
        CreateRoot();
    }

    public void CreateRoot(){
        if(items != null){
            Destroy(items);
        }

        items = new GameObject();
        items.transform.parent = mapPivot;
        items.transform.localPosition = Vector3.zero;
        items.name = "Items Container";
    }

    public void LoadMap(Dictionary<string, SceneGameObject> loadedData){
        CreateRoot();

        Debug.LogWarning("Reimplement map loader");
    }

}