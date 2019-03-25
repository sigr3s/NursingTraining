
using UnityEngine;

namespace NT.SceneObjects
{
    [System.Serializable]
    public struct UISceneObject{
        public Color color;
        public Sprite icon;
    }

    [System.Serializable]
    public struct SceneGameObject{
        public LayerMask canBePlacedOver;
        public GameObject model;

    }

}