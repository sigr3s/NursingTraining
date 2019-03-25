
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{

    public class NTSceneObject<T> : NTVariable<T>, INTSceneObject
    {

        public string sceneObject;

        public NTSceneObject()
        {
        }

        public override T DeserializeValue(string data){ 
            if(string.IsNullOrEmpty(data)){ 
                return default(T);
            }
            SerializationHelper sh =  JsonUtility.FromJson<SerializationHelper>(data);
            
            sceneObject = sh.sceneObjectGUID;
            
            return sh.value;  
         }

        public override string SerializeValue(T val){
            SerializationHelper sh = new SerializationHelper{value = val, sceneObjectGUID = sceneObject};

            return JsonUtility.ToJson(sh); 
        }

        public void SetName(string name)
        {
            SetKey(name);
        }

        public void SetScriptableObject(string guid)
        {
            sceneObject = guid;
        }

        private struct SerializationHelper{
            public string sceneObjectGUID;
            public T value;
        }

        
    }

}