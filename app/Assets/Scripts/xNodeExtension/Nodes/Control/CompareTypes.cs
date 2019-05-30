using NT.Atributes;
using NT.SceneObjects;
using UnityEngine;
using XNode;

namespace NT.Nodes.Other {

    public class CompareTypes : NTNode {

        [Input(ShowBackingValue.Never, ConnectionType.Override)] public SceneGameObjectReference scneObject;
        
        [NTOutput] public bool result;

        [NTInput] public Tools tool;



        public override object GetValue(NodePort port) {
            if(port.fieldName == nameof(result) ){
                SceneGameObject scgo = GetInputValue<SceneGameObject>(nameof(scneObject), null);

                if(scgo != null && scgo is ITool){
                    ITool t = (ITool) scgo;

                    Debug.Log( t + " __ " + tool);

                    return t?.GetToolType() == tool;
                }
                else
                {
                    Debug.Log( "WOWW¿¿   ___ " + scgo);

                    return false;
                }
            }
            else
            {
                return null;
            }
        }

        public override string GetDisplayName(){
            return "Object is type of";
        }

        public override int GetWidth(){
            return 300;
        }
    }
}
