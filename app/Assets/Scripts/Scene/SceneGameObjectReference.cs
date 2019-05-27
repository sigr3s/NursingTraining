using UnityEngine;

namespace NT.SceneObjects
{
    public class SceneGameObjectReference{
        private SceneGameObject _reference;

        [HideInInspector] 
        public SceneGameObject reference {
            get{
                if(_reference == null && !string.IsNullOrEmpty(linkedSGO)){
                    _reference = SessionManager.Instance.GetSceneGameObject(linkedSGO);
                }

                return _reference;
            }

            set{
                if(value != null){
                    _reference = value;
                    linkedSGO = value.data.id;
                }
            }
        }

        [SerializeField] public string linkedSGO;

        public SceneGameObjectReference(SceneGameObject reference)
        {
            this.reference = reference;
        }
    } 
}