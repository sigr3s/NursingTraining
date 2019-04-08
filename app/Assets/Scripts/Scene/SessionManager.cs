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

    [SerializeField] private Dictionary<string, SceneObjectCollider> sceneObjects;    
    [SerializeField] private SceneObjectCollider _selectedObjectSceneObject;
    public SceneObjectCollider selectedSceneObject{
        get{
            return _selectedObjectSceneObject;
        }

        private set{
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
    [HideInInspector] public UnityEvent OnSceneChanged = new UnityEvent();

    private void Awake() {
        if(sceneGraph == null){
            sceneGraph = (SceneGraph) ScriptableObject.CreateInstance(typeof(SceneGraph));
        }

        if(_sceneVariables == null){
            _sceneVariables = (SceneVariables) ScriptableObject.CreateInstance(typeof(SceneVariables));
            _sceneVariables.variableRepository = new NTVariableRepository();
        }

        sceneObjects = new Dictionary<string, SceneObjectCollider>();


        if(loadOnAwake){
           LoadSession(SessionData.sessionID);
        }
    }
    
    [ContextMenu("Save")]
    public void SaveSession(){
        if(string.IsNullOrEmpty(SessionData.sessionID)){
            SessionData.sessionID = DateTime.Now.ToString();
        }

        Debug.Log( $" Saving session to: {Application.persistentDataPath} ");

        string saveFolder = Application.dataPath + "/Saves/" + SessionData.sessionID;
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
        File.WriteAllText(saveFolder + "/" + "config.json", sessionJSON);
    }

    public void LoadSession(string sessionID){

        string saveFolder = Application.dataPath + "/Saves/" + sessionID;

        if(string.IsNullOrEmpty(sessionID)) return;

        if(!Directory.Exists(saveFolder) ){
            return;
        }

        string configJSON = File.ReadAllText(saveFolder + "/" + "config.json");
        SessionData = JsonUtility.FromJson<SessionData>(configJSON); 

        string variablesJSON = File.ReadAllText(saveFolder +  "/" + SessionData.variablesFile);
        JsonUtility.FromJsonOverwrite(variablesJSON, _sceneVariables);
        _sceneVariables.variableRepository.dictionary.OnAfterDeserialize();

        sceneGraph.Import(saveFolder +  "/" + SessionData.sceneGraphFile);
        sceneGraph.sceneVariables = _sceneVariables;

        
        //JSONNode variableRoot = JSON.Parse(variablesJSON);
        //_sceneVariables = SimpleJSONExtension.FromJSON<SceneVariables>(variableRoot);
        
        //Assign variables to graphs!


        //Rebuild all

        //sceneVariables.variableRepository.dictionary.OnAfterDeserialize();
        sceneObjects = new Dictionary<string, SceneObjectCollider>();

        OnSessionLoaded.Invoke();
    }

    public void AddSceneObject(SceneObjectCollider so){
        sceneObjects.Add(so.NTKey, so);
        OnSceneChanged.Invoke();
    }

    public void Remove(string key){

        if(sceneObjects.ContainsKey(key)){
            selectedSceneObject = sceneObjects[key];
            OnSceneChanged.Invoke();
        }
    }

    public void SetSelected(string key){

        if(selectedSceneObject != null) selectedSceneObject.isSelected = false;

        if(sceneObjects.ContainsKey(key)){
            selectedSceneObject = sceneObjects[key];
            selectedSceneObject.isSelected = true;
        }
        else
        {
            selectedSceneObject = null;
        }
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