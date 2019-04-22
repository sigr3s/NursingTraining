using System;
using System.Collections.Generic;
using System.IO;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public class PrefabObject : SceneObject{
        public SavedPrefab prefab;
        public GameObject craftedPrefab;
        public Dictionary<string, NTVariableData> childsData = new Dictionary<string, NTVariableData>();

        public static string exportPath = Application.dataPath + "/saves/Prefabs/";


        public override GameObject GetPreviewGameObject(){

            if(craftedPrefab != null){
                var rg = GameObject.Instantiate(craftedPrefab);
                rg.SetActive(true);
                return rg;
            }

                        
            //FIXME: This depends on the session manager... 
            // Ideally this should work kind of alone with the scene objects

            SceneObject rootSo = SessionManager.Instance.sceneObjects.GetObject(prefab.root.scneObjectGUID);

            if(rootSo != null){
                sceneGameObject.canBePlacedOver = rootSo.GetLayerMask();

                craftedPrefab = rootSo.GetPreviewGameObject();

                SceneGameObject parentSGO = craftedPrefab.GetComponent<SceneGameObject>();

                if(parentSGO == null){
                    parentSGO = craftedPrefab.AddComponent<SceneGameObject>();
                }

                parentSGO.NTDataType = rootSo.GetDataType();
                parentSGO.sceneObject = rootSo;

                Dictionary<string, GameObject> parentsGO = new Dictionary<string, GameObject>();
                Dictionary<string, List<GameObject>> childsGO = new Dictionary<string, List<GameObject>>();

                parentsGO.Add(prefab.root.id, craftedPrefab);

                foreach(PrefabSceneObject childPrefab in prefab.prefabObjects){
                    GameObject childGO = null;

                    SceneObject childSo = SessionManager.Instance.sceneObjects.GetObject(childPrefab.scneObjectGUID);

                    if(childSo == null){
                        Debug.LogWarning("Broken prefab!!!");
                        continue;
                    } 

                    childGO = childSo.GetPreviewGameObject();

                    SceneGameObject scgo = childGO.GetComponent<SceneGameObject>();

                    if(scgo == null){
                        scgo = childGO.AddComponent<SceneGameObject>();
                    }

                    scgo.NTDataType = childSo.GetDataType();
                    scgo.sceneObject = childSo;


                    if(childsGO.ContainsKey(childPrefab.id)){
                        List<GameObject> childsOfChild = childsGO[childPrefab.id];

                        foreach(GameObject childOfChild in childsOfChild){
                            Vector3 loclPos = childOfChild.transform.localPosition;
                            Quaternion loclRot = childOfChild.transform.localRotation;

                            //Recover local pos / rotation
                            childOfChild.transform.SetParent(childGO.transform);
                            childOfChild.transform.localScale = Vector3.one;
                            childOfChild.transform.localPosition = loclPos;
                            childOfChild.transform.localRotation = loclRot;
                        }

                        childsGO.Remove(childPrefab.id);
                    } 

                    if(parentsGO.ContainsKey(childPrefab.parent)){
                        GameObject parent = parentsGO[childPrefab.parent];
                        childGO.transform.SetParent(parent.transform);

                        childGO.transform.localPosition = childPrefab.localPosition;
                        childGO.transform.localRotation = Quaternion.Euler(childPrefab.localRotation);
                    }   
                    else
                    {
                        childGO.transform.localPosition = childPrefab.localPosition;
                        childGO.transform.localRotation = Quaternion.Euler(childPrefab.localRotation);

                        if(childsGO.ContainsKey(childPrefab.parent)){
                            List<GameObject> parentChilds = childsGO[childPrefab.parent];
                            parentChilds.Add(childGO);
                            childsGO[childPrefab.parent] = parentChilds;
                        }
                        else
                        {
                            List<GameObject> parentChilds = new List<GameObject>(){childGO};
                            childsGO.Add(childPrefab.parent, parentChilds); 
                        }
                    }

                    parentsGO.Add(childPrefab.id, childGO);   
             
                }

            }

            craftedPrefab.SetActive(false);
            craftedPrefab.name = "Carfted Prefab -- " + prefab.root.id;
            var retGO = GameObject.Instantiate(craftedPrefab);
            retGO.SetActive(true);
            return retGO;

        }

        public static PrefabObject LoadPrefab(string prefabFile){
            PrefabObject loadedPrefab = ScriptableObject.CreateInstance<PrefabObject>();
            if(File.Exists(prefabFile)){
                string loadedPrefabJSON = File.ReadAllText(prefabFile);
                loadedPrefab.prefab = JsonUtility.FromJson<SavedPrefab>(loadedPrefabJSON);
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
            GameObject instancedGo = GameObject.Instantiate(craftedPrefab, parent);
            instancedGo.SetActive(true);
            instancedGo.transform.localPosition = localPosition;
            instancedGo.transform.localRotation = localRotation;

            SceneGameObject scgo = instancedGo.GetComponent<SceneGameObject>();

            if(scgo == null){
                scgo = instancedGo.AddComponent<SceneGameObject>();
            }

            //FIXME: WTF
            //Needs only key assignement and recovery of all data :O
            //scgo.NTKey = ;

            return scgo;
        }

        public override SceneGameObject Instantiate(
            NTVariableRepository repository, Transform parent,
            Vector3 localPosition, Quaternion localRotation
        ){
            return Instantiate("", parent, localPosition, localRotation);
        }


        public static bool CreatePrefab(string prefabID, SceneGameObject root){
            if(string.IsNullOrEmpty(prefabID)){
                return false;
            }

            Debug.Log("Create prefab? ____ " + prefabID);

            string prefabFolder = exportPath + "/" ;

            if(!Directory.Exists(exportPath)){
                Directory.CreateDirectory(prefabFolder);
            }


            SavedPrefab savedPrefab = new SavedPrefab {
                GUID = Guid.NewGuid().ToString(),
                prefabObjects = new List<PrefabSceneObject>()
            };

            Debug.Log("root is prefab: " + root.sceneObject.GetGUID(), root);

            PrefabSceneObject prefabRoot = new PrefabSceneObject();
            prefabRoot.scneObjectGUID = root.sceneObject.GetGUID();
            prefabRoot.id = root.NTKey;
            prefabRoot.localPosition = root.transform.localPosition;
            prefabRoot.localRotation = root.transform.localRotation.eulerAngles;

            //Get NT DATA of the root
            NTVariable ntData = (NTVariable) SessionManager.Instance.sceneVariables.variableRepository.GetNTValue(root.NTKey, root.NTDataType);

            NTVariableData  ntRootVarData = ntData.ToNTVariableData();
            prefabRoot.serializedNTData = JsonUtility.ToJson(ntRootVarData);

            if(root.graph != null){
                prefabRoot.serializedGraph = root.graph.ExportSerialized();
            }

            savedPrefab.root = prefabRoot;


            List<SceneGameObject> childs = new List<SceneGameObject>(root.GetComponentsInChildren<SceneGameObject>());


            foreach(SceneGameObject sgo in childs){
                if(sgo.parent == null){
                    Debug.Log("No parent? ", sgo);
                    continue;
                }

                PrefabSceneObject childPrefab = new PrefabSceneObject();
                childPrefab.id = sgo.NTKey;
                childPrefab.parent = sgo.parent.NTKey;
                childPrefab.scneObjectGUID = sgo.sceneObject.GetGUID();
                childPrefab.localPosition = sgo.transform.localPosition;
                childPrefab.localRotation = sgo.transform.localRotation.eulerAngles;

                NTVariable childNtData = (NTVariable) SessionManager.Instance.sceneVariables.variableRepository.GetNTValue(sgo.NTKey, sgo.NTDataType);
                NTVariableData  childntRootVarData = childNtData.ToNTVariableData();

                childPrefab.serializedNTData = JsonUtility.ToJson(ntRootVarData);

                if(sgo.graph != null){
                    childPrefab.serializedGraph = sgo.graph.ExportSerialized();
                }

                savedPrefab.prefabObjects.Add(childPrefab);
            }

            string exportJSON = JsonUtility.ToJson(savedPrefab);

            File.WriteAllText(exportPath + "/" + prefabID + ".json" , exportJSON);

            return true;
        }

        [System.Serializable]
        public struct SavedPrefab{
            public string GUID;
            public PrefabSceneObject root;
            public List<PrefabSceneObject> prefabObjects;
        }

        [System.Serializable]
        public struct PrefabSceneObject{
            public string id; // This will be NTKey when saving as it is already uniquie
            public string scneObjectGUID; // Scene Object GUID
            public string serializedNTData; // NT data
            public string serializedGraph; // Serialized Graph
            public string parent; //Link to parent

            public Vector3 localPosition;
            public Vector3 localRotation;
        }


    }

}