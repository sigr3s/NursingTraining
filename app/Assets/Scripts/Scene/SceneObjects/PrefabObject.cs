using System;
using System.Collections.Generic;
using System.IO;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public class PrefabObject : SceneObject{

        static string exportPath = Application.dataPath + "/saves/prefabs/";

        public override Type GetDataType(){
            return typeof(string);
        }

        public override GameObject GetPreviewGameObject(){
            return new GameObject();
        }


        public static bool CreatePrefab(string prefabID, SceneGameObject root){
            if(string.IsNullOrEmpty(prefabID)){
                return false;
            }

            string prefabFolder = exportPath + "/" ;

            if(Directory.Exists(exportPath)){
                Directory.Delete(prefabFolder, true);
            }

            Directory.CreateDirectory(prefabFolder);

            SavedPrefab savedPrefab = new SavedPrefab {
                GUID = Guid.NewGuid().ToString(),
                prefabObjects = new List<PrefabSceneObject>()
            };

            PrefabSceneObject prefabRoot = new PrefabSceneObject();
            prefabRoot.scneObjectGUID = root.sceneObject.GetGUID();
            prefabRoot.id = root.NTKey;

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

        }


    }

}