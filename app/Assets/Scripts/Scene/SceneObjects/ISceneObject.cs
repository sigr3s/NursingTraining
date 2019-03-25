using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{
    public interface ISceneObject {
        List<Type> GetCompatibleNodes();
        List<string> GetCallbacks();    
        SceneGameObject GetSceneGameObject();
        UISceneObject GetUISceneObject();
        GameObject GetModel();
        LayerMask GetLayerMask();
        string GetName();
        void SetName(string name);
        ScriptableObject GetScriptableObject();
        string GetGUID();
        Type GetDataType();
    }
}