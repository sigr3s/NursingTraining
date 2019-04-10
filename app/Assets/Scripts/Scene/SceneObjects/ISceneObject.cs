using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{
    public interface ISceneObject {
        List<string> GetCallbacks();    
        UISceneObject GetUISceneObject();
        GameObject GetModel();
        LayerMask GetLayerMask();
        string GetName();
        string GetGUID();
        bool CanHoldItem(SceneGameObject obj);
        Type GetDataType();
    }
}