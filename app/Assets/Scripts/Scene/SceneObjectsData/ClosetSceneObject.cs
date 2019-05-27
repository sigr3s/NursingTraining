using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{
    public struct ClosetData{
        public Drawer mainDrawer;
    }

    [System.Serializable]
    public struct Drawer{
        public bool canBeOpened;
        public SceneGameObjectReference slot00;
        public SceneGameObjectReference slot01;
        public SceneGameObjectReference slot02;
        public SceneGameObjectReference slot03;
        public SceneGameObjectReference slot04;
        public SceneGameObjectReference slot05;
    }

    [CreateAssetMenu(fileName = "BedSceneObject", menuName = "NT/Scene/Closet")]
    public class ClosetSceneObject : SceneObject<ClosetData> {
        public override List<string> GetCallbacks(){
            return new List<string>(){"OnItemDropped","OnItemGetted"};
        }

        public override bool CanHoldItem(SceneGameObject obj){
            return true;
        }

        public override void HoldItem(SceneGameObject obj){
            obj.gameObject.SetActive(false);
        }
    }
}