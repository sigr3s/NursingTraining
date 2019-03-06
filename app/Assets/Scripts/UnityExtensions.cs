using System;
using UnityEngine;
 
public static class UnityExtensions
{
    public static void RunOnChildrenRecursive(this GameObject go, Action<GameObject> action) {
        if (go == null) return;
        foreach (var trans in go.GetComponentsInChildren<Transform>(true)) {
            action(trans.gameObject);
        }
    }
}