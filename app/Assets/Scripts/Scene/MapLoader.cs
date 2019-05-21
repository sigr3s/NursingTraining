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

        Dictionary<string, SceneGameObject> parentsGO = new Dictionary<string, SceneGameObject>();
            Dictionary<string, List<SceneGameObject>> childsGO = new Dictionary<string, List<SceneGameObject>>();

        foreach (var item in loadedData)
        {
            SceneGameObject prefabElement = item.Value;

            GameObject prefabElementGO = null;

            SceneObject prefabElementSO = SessionManager.Instance.sceneObjects.GetObject(prefabElement.data.sceneObjectGUID);

            if(prefabElementSO == null){
                Debug.LogWarning("Broken prefab!!!");
                continue;
            }

            prefabElementGO = prefabElementSO.GetPreviewGameObject();

            SceneGameObject prefabElementSCGO = prefabElementGO.GetComponent<SceneGameObject>();

            if(prefabElementSCGO == null){
                prefabElementSCGO = prefabElementGO.AddComponent<SceneGameObject>();
            }
            
            prefabElementSCGO.sceneObject = prefabElementSO;

            SceneGameObjectData prefabElementInstanceData = prefabElement.data;
            prefabElementInstanceData.childs = new List<string>();
            prefabElementInstanceData.parent = "";

            prefabElementSCGO.LoadFromData(prefabElementInstanceData);

            if(childsGO.ContainsKey(prefabElement.data.id)){
                List<SceneGameObject> childsOfElement = childsGO[prefabElement.data.id];

                foreach(SceneGameObject childOfElement in childsOfElement){
                    childOfElement.transform.SetParent(prefabElementGO.transform);

                    childOfElement.RestoreTransform();

                    prefabElementSCGO.sceneObject.HoldItem(childOfElement, prefabElementSCGO);

                    childOfElement.data.parent = prefabElementSCGO.data.id;
                    prefabElementSCGO.data.childs.Add(childOfElement.data.id);
                }

                childsGO.Remove(prefabElement.data.id);
            }

            if(string.IsNullOrEmpty(prefabElement.data.parent)){
                prefabElementGO.transform.SetParent(items.transform);
                prefabElementSCGO.RestoreTransform();
            }
            else if(parentsGO.ContainsKey(prefabElement.data.parent)){
                SceneGameObject parent = parentsGO[prefabElement.data.parent];

                parent.data.childs.Add(prefabElementSCGO.data.id);
                prefabElementSCGO.data.parent = parent.data.id;

                prefabElementSCGO.transform.SetParent(parent.transform);
                prefabElementSCGO.RestoreTransform();

                parent.sceneObject.HoldItem(prefabElementSCGO, parent);

            }   
            else
            {
                if(childsGO.ContainsKey(prefabElement.data.parent)){
                    List<SceneGameObject> parentChilds = childsGO[prefabElement.data.parent];
                    parentChilds.Add(prefabElementSCGO);
                    childsGO[prefabElement.data.parent] = parentChilds;
                }
                else
                {
                    List<SceneGameObject> parentChilds = new List<SceneGameObject>(){prefabElementSCGO};
                    childsGO.Add(prefabElement.data.parent, parentChilds); 
                }
            }

            
            
            SessionManager.Instance.AddSceneGameObject(prefabElementSCGO);
            parentsGO.Add(prefabElement.data.id, prefabElementSCGO);
            

        }
    }

    public virtual void ReloadUI()
    {
    }
}