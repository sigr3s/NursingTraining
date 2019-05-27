using System;
using System.Collections.Generic;
using System.IO;
using NT.Variables;
using OdinSerializer;
using UnityEngine;

namespace NT.SceneObjects
{
    public class PrefabObject : SceneObject{
        public SavedPrefab prefab;
        public static string exportPath = Application.dataPath + "/Saves/Prefabs/";
        public static DataFormat dataFormat = DataFormat.JSON;


        public GameObject CraftPrefab(Transform rootParent){
            SceneGameObject root = null;

            Dictionary<string, SceneGameObject> parentsGO = new Dictionary<string, SceneGameObject>();
            Dictionary<string, List<SceneGameObject>> childsGO = new Dictionary<string, List<SceneGameObject>>();


            foreach(SceneGameObject prefabElement in prefab.prefabObjects){
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

                prefabElementInstanceData.id = prefabElementSO.name + Guid.NewGuid().ToString();
                prefabElementInstanceData.childs = new List<string>();
                prefabElementInstanceData.parent = "";

                prefabElementSCGO.LoadFromData(prefabElementInstanceData);

                if(childsGO.ContainsKey(prefabElement.data.id)){
                    List<SceneGameObject> childsOfElement = childsGO[prefabElement.data.id];

                    foreach(SceneGameObject childOfElement in childsOfElement){
                        childOfElement.transform.SetParent(prefabElementGO.transform);
                        childOfElement.RestoreTransform();

                        prefabElementSCGO.HoldItem(childOfElement);


                        childOfElement.data.parent = prefabElementSCGO.data.id;
                        prefabElementSCGO.data.childs.Add(childOfElement.data.id);
                    }

                    childsGO.Remove(prefabElement.data.id);
                }

                if(prefabElement.data.id == prefab.root){
                    root = prefabElementSCGO;
                    this.sceneGameObject.canBePlacedOver = prefabElementSO.GetLayerMask();
                }
                else if(parentsGO.ContainsKey(prefabElement.data.parent)){
                    SceneGameObject parent = parentsGO[prefabElement.data.parent];

                    parent.data.childs.Add(prefabElementSCGO.data.id);
                    prefabElementSCGO.data.parent = parent.data.id;

                    prefabElementSCGO.transform.SetParent(parent.transform);
                    prefabElementSCGO.RestoreTransform();

                    parent.HoldItem(prefabElementSCGO);
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
                

                parentsGO.Add(prefabElement.data.id, prefabElementSCGO);
            
            }

            if(rootParent != null){
                root.transform.SetParent(rootParent);
                root.RestoreTransform();
            }

            return root.gameObject;
        }

        public override GameObject GetPreviewGameObject(){
            return CraftPrefab(null);
        }

        public static PrefabObject LoadPrefab(string prefabFile){
            PrefabObject loadedPrefab = ScriptableObject.CreateInstance<PrefabObject>();

            if(File.Exists(prefabFile)){
                byte[] loadedPrefabData = File.ReadAllBytes(prefabFile);
                loadedPrefab.prefab = SerializationUtility.DeserializeValue<SavedPrefab>(loadedPrefabData, dataFormat);
            }
            else
            {
                return null;
            }

            return loadedPrefab;
        }

        public override SceneGameObject Instantiate(string key, Transform parent,
            Vector3 localPosition, Quaternion localRotation
        ){
            GameObject instancedGo = CraftPrefab(parent);

            instancedGo.SetActive(true);
            instancedGo.transform.localPosition = localPosition;
            instancedGo.transform.localRotation = localRotation;

            SceneGameObject scgo = instancedGo.GetComponent<SceneGameObject>();

            if(scgo == null){
               Debug.LogError("Fatal error on prefab!");
            }

            SceneGameObject[] prefabSCGOs = instancedGo.GetComponentsInChildren<SceneGameObject>(true);

            foreach(SceneGameObject prefabSCGO in prefabSCGOs){
                if(prefabSCGO == scgo) continue;

                SessionManager.Instance.AddSceneGameObject(prefabSCGO);
            }

            return scgo;
        }

        public override SceneGameObject Instantiate( Transform parent,
            Vector3 localPosition, Quaternion localRotation
        ){
            return Instantiate("", parent, localPosition, localRotation);
        }


        public static bool CreatePrefab(string prefabID, SceneGameObject root, string name = "", int sprite = 0){
            if(string.IsNullOrEmpty(prefabID)){
                return false;
            }

            string prefabFolder = exportPath + "/" ;

            if(!Directory.Exists(exportPath)){
                Directory.CreateDirectory(prefabFolder);
            }

            SavedPrefab savedPrefab = new SavedPrefab {
                name = name,
                sprite = sprite,
                root = root.data.id,
                prefabObjects = new List<SceneGameObject>(root.GetComponentsInChildren<SceneGameObject>(true))
            };

            byte[] exportData = SerializationUtility.SerializeValue(savedPrefab, dataFormat);

            File.WriteAllBytes(exportPath + "/" + prefabID + ".nt" , exportData);

            return true;
        }

        [System.Serializable]
        public struct SavedPrefab{
            public int sprite;
            public string name;
            public string root;
            public List<SceneGameObject> prefabObjects;
        }

    }

}