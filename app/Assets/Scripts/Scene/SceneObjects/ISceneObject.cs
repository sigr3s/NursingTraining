using System;
using System.Collections.Generic;
using NT.Variables;
using UnityEngine;

namespace NT.SceneObjects
{
    public interface ISceneObject {
        List<string> GetCallbacks();

        UISceneObject GetUISceneObject();
        GameObject GetPreviewGameObject();
        LayerMask GetLayerMask();

        string GetGUID();
        bool CanHoldItem(SceneGameObject obj);
        void HoldItem(SceneGameObject obj, SceneGameObject parent);

        SceneGameObject Instantiate(Transform parent,
            Vector3 localPosition, Quaternion localRotation
        );
        SceneGameObject Instantiate(
            string key, Transform parent,
            Vector3 localPosition, Quaternion localRotation
        );
    }
}