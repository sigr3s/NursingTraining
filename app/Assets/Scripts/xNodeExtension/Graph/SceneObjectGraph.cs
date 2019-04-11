using UnityEngine;

namespace NT.Graph{
    [CreateAssetMenu(fileName = "Scene Object Graph", menuName = "NT/Scene Object Graph")]
    public class SceneObjectGraph : NTGraph
    {
        public string linkedNTVariable;

        public override void LoadFromGraph(NTGraph g){
            base.LoadFromGraph(g);
            linkedNTVariable = ((SceneObjectGraph) g).linkedNTVariable;
        }
    }
}