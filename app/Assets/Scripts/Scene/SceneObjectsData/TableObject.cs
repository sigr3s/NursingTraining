using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{

    [System.Serializable]
    public struct TableData{
        public SceneGameObjectReference slot00;
        public SceneGameObjectReference slot01;
        public SceneGameObjectReference slot02;
        public SceneGameObjectReference slot03;
        public SceneGameObjectReference slot04;
    }

    [CreateAssetMenu(fileName = "TableObject", menuName = "NT/Scene/Table")]
    public class TableObject : SceneObject<TableData> {

        public override List<string> GetCallbacks()
        {
            return new List<string>() { "Cover On" };
        }

    }
}