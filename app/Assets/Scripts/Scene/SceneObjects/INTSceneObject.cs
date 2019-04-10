
using UnityEngine;

namespace NT.SceneObjects
{

    public interface INTSceneObject {
        void SetName(string name);
        void SetScriptableObject(string guid);
        void SetPosition(Vector3 position);
        void SetRotation(Vector3 rotation);

        SceneObjectExtraData GetExtraData();

    }

}