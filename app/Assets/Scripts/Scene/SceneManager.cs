using System.Collections.Generic;
using NT.Graph;
using NT.Variables;
using UnityEngine;

public class SceneManager : Singleton<SceneManager> {
    private SceneVariables _sceneVariables;

    public SceneVariables sceneVariables {
        get{
            if(_sceneVariables == null){
                _sceneVariables = (SceneVariables) ScriptableObject.CreateInstance(typeof(SceneVariables));
                _sceneVariables.variableRepository = new NTVariableRepository();
            }

            return _sceneVariables;
        }
    }

    public SceneGraph sceneGraph;
    public List<NTGraph> sceneObjectsGraph;

}