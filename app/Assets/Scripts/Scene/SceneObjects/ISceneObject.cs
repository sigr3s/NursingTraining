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

        SceneGameObject Instantiate(
            NTVariableRepository repository, Transform parent, 
            Vector3 localPosition, Quaternion localRotation
        );
        SceneGameObject Instantiate(
            string key, Transform parent, 
            Vector3 localPosition, Quaternion localRotation
        );
    }
}