using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{

    [CreateAssetMenu(fileName = "SceneObject", menuName = "NT/Object")]
    public class DummySceneObject : ScriptableObject, ISceneObject
    {   
        public Vector2 size;

        public GameObject model;



        public List<string> GetCallbacks()
        {
            throw new NotImplementedException();
        }

        public List<Type> GetCompatibleNodes()
        {
            throw new NotImplementedException();
        }

        public GameObject GetModel()
        {
            return model;
        }
    }
}