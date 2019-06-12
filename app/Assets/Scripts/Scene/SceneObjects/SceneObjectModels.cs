
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
        All = 0,
        Furniture = 1,
        Devices = 2,
        Tools = 3,
        Decoration = 4,
        UserPrefabs = 5
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