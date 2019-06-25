using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects{
    public struct TableClothData{

    }

    [CreateAssetMenu(fileName = "TableObject", menuName = "NT/Scene/Table")]
    public class TableClothObject : SceneObject<TableClothData> {
        public override List<string> GetCallbacks(){
            return new List<string>(){"On Table Cloth Placed"};
        }
    }
}