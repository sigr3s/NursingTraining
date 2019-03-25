

using System;
using System.Collections.Generic;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public class SceneObject<T> : SceneObject where T : INTSceneObject 
    {
        public override Type GetDataType()
        {
            return typeof(T);
        }
    }

    public class SceneObject : ScriptableObject, ISceneObject{
        public SceneGameObject sceneGameObject;
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

        public virtual List<Type> GetCompatibleNodes()
        {
             return new List<Type>();
        }

        public virtual SceneGameObject GetSceneGameObject()
        {
            return sceneGameObject;
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual UISceneObject GetUI(){
            return sceneObjectUI;
        }

        public GameObject GetModel()
        {
            return sceneGameObject.model;
        }

        public LayerMask GetLayerMask()
        {
            return sceneGameObject.canBePlacedOver;
        }

        public virtual ScriptableObject GetScriptableObject()
        {
            return this;
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

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}