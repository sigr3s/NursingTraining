using System;
using System.Collections.Generic;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public class SceneObject<T> : SceneObject
    {
        public override NTVariable GetDefaultData(){
            NTVariable savedSceneObject = (NTVariable) Activator.CreateInstance( typeof(NTSceneObject<T>) );
            return savedSceneObject;
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
            GameObject ig = GameObject.Instantiate(sceneGameObject.model);
            ig.name = ig.name.Replace("(Clone)", "");
            return ig;
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

        public virtual NTVariable GetDefaultData()
        {
            return null;
        }

        public virtual bool CanHoldItem(SceneGameObject obj)
        {
            return false;
        }


        public virtual void HoldItem(SceneGameObject obj, SceneGameObject parent){
            
        }

        public virtual SceneGameObject Instantiate(string key, Transform parent,
            Vector3 localPosition, Quaternion localRotation
        ){
            GameObject instancedGo = GameObject.Instantiate(sceneGameObject.model, parent);
            instancedGo.name = instancedGo.name.Replace("(Clone)", "");
            
            instancedGo.transform.localPosition = localPosition;
            instancedGo.transform.localRotation = localRotation;

            SceneGameObject scgo = instancedGo.GetComponent<SceneGameObject>();

            if(scgo == null){
                scgo = instancedGo.AddComponent<SceneGameObject>();
            }

            scgo.data.id = key;
            scgo.data.data =  GetDefaultData();

            SceneGameObject parentSCGO = parent.GetComponent<SceneGameObject>();

            if(parentSCGO != null){
                scgo.data.parent = parentSCGO.data.id;
                
                if(!parentSCGO.data.childs.Contains(key)){
                    parentSCGO.data.childs.Add(key);
                }
                else
                {
                    Debug.LogError("Something went wrong!");
                }
            }
            

            scgo.sceneObject = this;

            return scgo;
        }


        public virtual SceneGameObject Instantiate( Transform parent,
            Vector3 localPosition, Quaternion localRotation
        ){
            string key = name + Guid.NewGuid().ToString();            
            return Instantiate(key, parent, localPosition, localRotation);
        }
    }
}