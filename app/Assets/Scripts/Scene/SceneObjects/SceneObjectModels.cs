
using UnityEngine;

namespace NT.SceneObjects
{
    [System.Serializable]
    public struct UISceneObject{
        public Color color;
        public Sprite icon;
        public ObjectCategory category;
    }

    public enum ObjectCategory
    {
        UserPrefabs,
        Furniture,
        Props,
        Tools,
        Decoration
    }

    [System.Serializable]
    public struct SceneGameObjectInfo{
        public LayerMask canBePlacedOver;
        public GameObject model;

    }

    [System.Serializable]
    public struct SceneObjectExtraData{
        public string sceneObjectGUID;
        public Vector3 position;
        public Vector3 rotation;
    }

}