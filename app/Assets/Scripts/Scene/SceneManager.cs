using System.Collections.Generic;
using NT.Graph;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;
using UnityEngine.Events;

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

    private ISceneObject _currentObject;
    public ISceneObject currentObject{
        get{
            return _currentObject;
        }

        set{
            _currentObject = value;
            OnCurrentChanged.Invoke();
        }
    }

    public UnityEvent OnCurrentChanged = new UnityEvent();

    


    //Import export!

    public void SaveSession(){

    }


    public void LoadSession(){

    }



}