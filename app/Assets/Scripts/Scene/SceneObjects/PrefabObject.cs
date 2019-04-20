using System;
using System.Collections.Generic;
using System.IO;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public class PrefabObject : SceneObject{

        string exportPath = Application.dataPath + "/saves/prefabs/";

        public override Type GetDataType(){
            return typeof(string);
        }

        public override GameObject GetPreviewGameObject(){
            return new GameObject();
        }


        public bool CreatePrefab(string prefabID, SceneGameObject root){
            if(string.IsNullOrEmpty(prefabID)){
                return false;
            }

            string prefabFolder = exportPath + "/" + prefabID;

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

            //NTData can be saved and loaded!


            //

            string exportJSON = JsonUtility.ToJson(savedPrefab);

            File.WriteAllText(exportPath, exportJSON);

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