
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{

    public class NTSceneObject<T> : NTVariable<T>, INTSceneObject
    {

        public SceneObjectExtraData sceneObjectExtraData;

        public NTSceneObject()
        {
        }

        public override string SerializeExtraData()
        {
            return JsonUtility.ToJson(sceneObjectExtraData);
        }

        public override void DeserializeExtraData(string ExtraData){
            if(string.IsNullOrEmpty(ExtraData)) return;
            
            sceneObjectExtraData = JsonUtility.FromJson<SceneObjectExtraData>(ExtraData);
        }

        public void SetName(string name)
        {
            SetKey(name);
        }

        public void SetPosition(Vector3 position)
        {
            sceneObjectExtraData.position = position;
        }

        public void SetRotation(Vector3 rotation)
        {
            sceneObjectExtraData.rotation = rotation;
        }

        public void SetScriptableObject(string guid)
        {
            sceneObjectExtraData.sceneObjectGUID = guid;
        }

        public SceneObjectExtraData GetExtraData(){
            return sceneObjectExtraData;
        }
    }

}