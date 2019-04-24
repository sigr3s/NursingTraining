using System.Collections.Generic;
using NT.Graph;
using NT.SceneObjects;
using UnityEngine;

public class MapLoader : MonoBehaviour {
    public Transform mapPivot;
    public GameObject items;


    private void Start() {
        SessionManager.Instance.OnSessionLoaded.AddListener(LoadMap); 
        LoadMap();   
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

    public void LoadMap(){
        CreateRoot();

        var loadedScene = SessionManager.Instance.loadedScene;

        
        var childs = new Dictionary<string, List<SceneGameObject>>();

        foreach(var loadedSceneObject in loadedScene.objects){

            Transform itemParent = items.transform;

            SceneObject so = SessionManager.Instance.sceneObjects.GetObject(loadedSceneObject.ScriptableObjectGUID);

            if(so == null){
                Debug.LogWarning("Could not load SceneObject with this instance");
                continue;
            }

            SceneGameObject scgo =  so.Instantiate(loadedSceneObject.AssignedNTVariable, itemParent, loadedSceneObject.position, Quaternion.Euler(loadedSceneObject.rotation));

            if(!string.IsNullOrEmpty(loadedSceneObject.serializedGraph)){
                scgo.graph = ScriptableObject.CreateInstance<SceneObjectGraph>();
                scgo.graph.ImportSerialized(loadedSceneObject.serializedGraph);
                scgo.graph.sceneVariables = SessionManager.Instance.sceneVariables;
            }

            if(!string.IsNullOrEmpty(loadedSceneObject.parent)){
                SceneGameObject parentscgo = SessionManager.Instance.GetSceneGameObject(loadedSceneObject.parent);

                if(parentscgo != null){
                    scgo.transform.SetParent(parentscgo.transform);
                    scgo.transform.localScale = Vector3.one;
                    scgo.transform.localPosition = loadedSceneObject.position;
                    scgo.transform.localRotation = Quaternion.Euler(loadedSceneObject.rotation);

                    parentscgo.sceneObject.HoldItem(scgo, parentscgo);
                }
                else
                {
                    scgo.transform.localPosition = loadedSceneObject.position;
                    scgo.transform.localRotation = Quaternion.Euler(loadedSceneObject.rotation);

                    if(childs.ContainsKey(loadedSceneObject.parent)){
                        List<SceneGameObject> childsForObj = childs[loadedSceneObject.parent];
                        childsForObj.Add(scgo);
                        childs[loadedSceneObject.parent] = childsForObj;
                    }
                    else
                    {
                        childs.Add(loadedSceneObject.parent, new List<SceneGameObject>(){ scgo });
                    }
                }                
            }

            if(childs.ContainsKey(loadedSceneObject.AssignedNTVariable)){
                List<SceneGameObject> childsForObj = childs[loadedSceneObject.AssignedNTVariable];

                foreach(SceneGameObject sgo in childsForObj){
                    Vector3 localPos = sgo.transform.localPosition;
                    Quaternion loclRot = sgo.transform.localRotation;

                    sgo.transform.SetParent(scgo.transform);
                    sgo.transform.localScale = Vector3.one;
                    sgo.transform.localPosition = localPos;
                    sgo.transform.localRotation = loclRot;

                    scgo.sceneObject.HoldItem(sgo, scgo);
                }

                childs.Remove(loadedSceneObject.AssignedNTVariable);
            }

            SessionManager.Instance.AddSceneGameObject(scgo);
        }

        foreach(var ll in childs){
            Debug.Log("sksksksk   " + ll.Key);
        }
    }

}