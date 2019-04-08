using System;
using System.Collections.Generic;
using System.IO;
using NT.Graph;
using NT.SceneObjects;
using NT.Variables;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class SessionManager : Singleton<SessionManager> {

    [Header("Session Data")]
    public SessionData SessionData;
    public bool loadOnAwake = true;
    

    [Header("Scene Variables")]
    [SerializeField] private SceneVariables _sceneVariables;

    public SceneVariables sceneVariables {
        get{
            if(_sceneVariables == null){
                _sceneVariables = (SceneVariables) ScriptableObject.CreateInstance(typeof(SceneVariables));
                _sceneVariables.variableRepository = new NTVariableRepository();
            }

            return _sceneVariables;
        }
    }


    [Header("Graphs")]
    public SceneGraph sceneGraph;
    public List<NTGraph> sceneObjectsGraphs;


    [Space(20)]
    [Header("Debug")]

    [SerializeField] private SceneObjectCollider _selectedObjectSceneObject;
    public SceneObjectCollider selectedSceneObject{
        get{
            return _selectedObjectSceneObject;
        }

        set{
            _selectedObjectSceneObject = value;
            OnCurrentChanged.Invoke();
        }
    } 

    [SerializeField] private NTGraph _showingGraph;
    [SerializeField] public NTGraph showingGraph{
        get{
            return _showingGraph;
        }  

        set{
            _showingGraph = value;
        }
    }


    [HideInInspector] public UnityEvent OnCurrentChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnShowingGraphChanged = new UnityEvent();

    [HideInInspector] public UnityEvent OnSessionLoaded = new UnityEvent();

    private void Awake() {
        if(loadOnAwake){
           //LoadSession(SessionData.sessionID);
        }
        else
        {
            sceneGraph = (SceneGraph) ScriptableObject.CreateInstance(typeof(SceneGraph));
        }
    }
    
    [ContextMenu("Save")]
    public void SaveSession(){
        Debug.Log( $" Saving session to: {Application.persistentDataPath} ");

        string saveFolder = Application.dataPath + "/" + SessionData.sessionID;
        if(Directory.Exists(saveFolder) ){
            Directory.Delete(saveFolder, true);
        }

        Directory.CreateDirectory(saveFolder);


        SessionData.lastModified = DateTime.Now.ToString();
        SessionData.variablesFile = "variables.json";

        string sceneVariablesJSON = JsonUtility.ToJson(_sceneVariables);       
        File.WriteAllText(saveFolder + "/" + SessionData.variablesFile, sceneVariablesJSON);

        SessionData.sceneGraphFile = "sceneGraph.json";
        sceneGraph.Export(saveFolder + "/" + SessionData.sceneGraphFile);


        SessionData.objectsGraphsFiles = new List<string>();

        foreach(NTGraph objectGraph in sceneObjectsGraphs){
            objectGraph.Export(saveFolder + "/" + objectGraph.name + "json");
            SessionData.objectsGraphsFiles.Add(objectGraph.name + "json");
        }

        string sessionJSON = JsonUtility.ToJson(SessionData);       
        File.WriteAllText(saveFolder + "/" + "config.json", sceneVariablesJSON);
    }

    public void LoadSession(string sessionID){
        if(!Directory.Exists(Application.dataPath + "/" + sessionID) ){
            return;
        }

        //Load graphs!


        //Load variables


        string variablesJSON = File.ReadAllText(Application.dataPath + "/" + sessionID +  "/variables.json");
        JsonUtility.FromJsonOverwrite(variablesJSON, _sceneVariables);
        
        //JSONNode variableRoot = JSON.Parse(variablesJSON);
        //_sceneVariables = SimpleJSONExtension.FromJSON<SceneVariables>(variableRoot);
        
        //Assign variables to graphs!


        //Rebuild all

        //sceneVariables.variableRepository.dictionary.OnAfterDeserialize();

        OnSessionLoaded.Invoke();
    }
}

[System.Serializable]
public struct SessionData{
    public string sessionID;
    public string lastModified;
    public string variablesFile;

    public string sceneGraphFile;        
    public List<string> objectsGraphsFiles;
}