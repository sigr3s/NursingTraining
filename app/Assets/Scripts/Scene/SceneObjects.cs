using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneObjects", menuName = "app/SceneObjects", order = 0)]
public class SceneObjects : ScriptableObject {
    public List<SceneObject> objectSet;

}