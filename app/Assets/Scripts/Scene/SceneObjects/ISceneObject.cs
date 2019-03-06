using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT
{
    public interface ISceneObject {
        List<Type> GetCompatibleNodes();
        List<string> GetCallbacks();    
        GameObject GetModel();
    }
}