using System;
using System.Collections.Generic;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public class PrefabObject : SceneObject{

        public override Type GetDataType(){
            return typeof(string);
        }

        public override GameObject GetPreviewGameObject(){
            return new GameObject();
        }


    }

}