using System.Collections.Generic;
using NT.Graph;
using UnityEngine;


namespace NT.Variables
{
    [CreateAssetMenu(fileName = "SceneVariables", menuName = "NT/SceneVariables", order = 0)]
    public class SceneVariables : ScriptableObject {
        public List<StringVariable> strings;
        public List<FloatVariable> floats;
        public List<BooleanVariable> bools;
        public List<IntegerVariable> integers;

        public SceneGraph sceneGraph;


        public void Initialize(){

        }
    }
}
