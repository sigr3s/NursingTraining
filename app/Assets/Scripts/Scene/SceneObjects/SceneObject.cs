using System;
using System.Collections.Generic;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public class SceneObject<T> : SceneObject
    {
        public override Type GetDataType()
        {
            return typeof(NTSceneObject<T>);
        }
    }

    public class SceneObject : ScriptableObject, ISceneObject{
        public SceneGameObjectInfo sceneGameObject;
        public UISceneObject sceneObjectUI;
        [SerializeField] private string GUID;


        public SceneObject(){
            GUID = Guid.NewGuid().ToString();
        }

        private void OnEnable() {
            if(string.IsNullOrEmpty(GUID)) GUID = Guid.NewGuid().ToString();
        }


        public virtual List<string> GetCallbacks()
        {
            return new List<string>();
        }


        public virtual UISceneObject GetUI(){
            return sceneObjectUI;
        }

        public virtual GameObject GetPreviewGameObject()
        {
            return GameObject.Instantiate(sceneGameObject.model);
        }

        public virtual LayerMask GetLayerMask()
        {
            return sceneGameObject.canBePlacedOver;
        }

        public string GetGUID()
        {
            if(string.IsNullOrEmpty(GUID)) GUID = Guid.NewGuid().ToString();

            return GUID;
        }

        public UISceneObject GetUISceneObject()
        {
            return sceneObjectUI;
        }

        public virtual Type GetDataType()
        {
            return null;
        }

        public virtual bool CanHoldItem(SceneGameObject obj)
        {
            return false;
        }

        public virtual SceneGameObject Instantiate(string key, Transform parent,
            Vector3 localPosition, Quaternion localRotation
        ){
            GameObject instancedGo = GameObject.Instantiate(sceneGameObject.model, parent);
            instancedGo.transform.localPosition = localPosition;
            instancedGo.transform.localRotation = localRotation;


            SceneGameObject scgo = instancedGo.GetComponent<SceneGameObject>();

            if(scgo == null){
                scgo = instancedGo.AddComponent<SceneGameObject>();
            }

            scgo.NTDataType = GetDataType();
            scgo.NTKey = key;

            scgo.sceneObject = this;

            return scgo;
        }


        public virtual SceneGameObject Instantiate(
            NTVariableRepository repository, Transform parent,
            Vector3 localPosition, Quaternion localRotation
        ){
            Type t = GetDataType();
            string key = name + Guid.NewGuid().ToString();

            INTSceneObject savedSceneObject = (INTSceneObject) Activator.CreateInstance(t);
            savedSceneObject.SetName(key);
            repository.AddVariable(t, savedSceneObject);

            return Instantiate(key, parent,localPosition, localRotation);
        }
    }
}