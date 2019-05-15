using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct ClosetData{
        public Drawer mainDrawer;
    }

    [System.Serializable]
    public struct Drawer{
        public bool canbeOpened;
        public SceneGameObject slot00;
        public SceneGameObject slot01;
        public SceneGameObject slot02;
        public SceneGameObject slot03;
        public SceneGameObject slot04;
        public SceneGameObject slot05;
    }

    [CreateAssetMenu(fileName = "BedSceneObject", menuName = "NT/Scene/Closet")]
    public class ClosetSceneObject : SceneObject<ClosetData> {
        public override List<string> GetCallbacks(){
            return new List<string>(){"OnItemDropped","OnItemGetted"};
        }

        public override bool CanHoldItem(SceneGameObject obj){
            return true;
        }

        public override void HoldItem(SceneGameObject obj, SceneGameObject parent){
            Debug.Log("Handele item???");
            obj.gameObject.SetActive(false);
            
            ClosetData cd = (ClosetData) parent.data.data.GetDefaultValue();
            cd.mainDrawer.slot00 = obj;
            parent.data.data.SetDefaultValue(cd);
        }
    }
}