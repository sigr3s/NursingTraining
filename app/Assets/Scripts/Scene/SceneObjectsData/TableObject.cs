using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct TebleData{
        public SceneGameObjectReference slot00;
        public SceneGameObjectReference slot01;
        public SceneGameObjectReference slot02;
        public SceneGameObjectReference slot03;
        public SceneGameObjectReference slot04;
        public SceneGameObjectReference slot05;
    }

    [CreateAssetMenu(fileName = "TableObject", menuName = "NT/Scene/Table")]
    public class TableObject : SceneObject<TebleData> {

        
        
    }
}