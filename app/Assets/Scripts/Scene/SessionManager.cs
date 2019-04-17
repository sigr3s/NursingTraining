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
    public List<SceneObjectGraph> sceneObjectsGraphs;


    [Space(20)]
    [Header("Debug")]

    [SerializeField] private Dictionary<string, SceneGameObject> sceneGameObjects;
    [SerializeField] private SceneGameObject _selectedObjectSceneObject;
    public SceneGameObject selectedSceneObject{
        get{
            return _selectedObjectSceneObject;
        }

        private set{
            if(_selectedObjectSceneObject != value){
                _selectedObjectSceneObject = value;
                OnCurrentChanged.Invoke();
            }
        }
    } 

    [SerializeField] private NTGraph _showingGraph;
    [SerializeField] public NTGraph showingGraph{
        get{
            return _showingGraph;
        }  

        set{
            if(_showingGraph != value){
                _showingGraph = value;
                OnShowingGraphChanged.Invoke();
            }
        }
    }


    [HideInInspector] public UnityEvent OnCurrentChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnShowingGraphChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnGraphListChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnSessionLoaded = new UnityEvent();
    [HideInInspector] public UnityEvent OnSceneGameObjectsChanged = new UnityEvent();

    private void Awake() {
        if(sceneGraph == null){
            sceneGraph = (SceneGraph) ScriptableObject.CreateInstance(typeof(SceneGraph));
        }

        if(_sceneVariables == null){
            _sceneVariables = (SceneVariables) ScriptableObject.CreateInstance(typeof(SceneVariables));
            _sceneVariables.variableRepository = new NTVariableRepository();
        }

        sceneGameObjects = new Dictionary<string, SceneGameObject>();


        if(loadOnAwake){
           LoadSession(SessionData.sessionID);
        }
    }

    [System.Serializable]
    public struct SavedScene{
        public List<SavedObject> objects;
    }

    [System.Serializable]
    public struct SavedObject{
        public string ScriptableObjectGUID;
        public string AssignedNTVariable;
        public Vector3 position;
        public Vector3 rotation;
        public List<string> childs;
        public string serializedGraph;
    }


    [ContextMenu("Scene save")]
    public void NewSceneSave(){
        
        SavedScene savedScene = new SavedScene(){ objects = new List<SavedObject>()};

        foreach(var sceneGameObject in sceneGameObjects){

            SavedObject savedObject = new SavedObject(){
                ScriptableObjectGUID = sceneGameObject.Value.sceneObject.GetGUID(),
                AssignedNTVariable = sceneGameObject.Key,
                position = sceneGameObject.Value.gameObject.transform.position,
                rotation = sceneGameObject.Value.gameObject.transform.localRotation.eulerAngles
            };

            for(int i = 0; i < sceneObjectsGraphs.Count; i++){
                if(sceneObjectsGraphs[i].linkedNTVariable == sceneGameObject.Key){
                    savedObject.serializedGraph = sceneObjectsGraphs[i].ExportSerialized(); 
                }
            }

            savedScene.objects.Add(savedObject);
        }

        Debug.Log( JsonUtility.ToJson(savedScene) );
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

        foreach(SceneObjectGraph objectGraph in sceneObjectsGraphs){
            objectGraph.Export(saveFolder + "/" + objectGraph.linkedNTVariable + ".json");
            SessionData.objectsGraphsFiles.Add(objectGraph.linkedNTVariable + ".json");
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


        sceneObjectsGraphs = new List<SceneObjectGraph>();
        foreach(string sceneObjectGraphFile in SessionData.objectsGraphsFiles){
            SceneObjectGraph soc = ScriptableObject.CreateInstance<SceneObjectGraph>();
            soc.Import(saveFolder + "/" + sceneObjectGraphFile);
            soc.sceneVariables = _sceneVariables;

            sceneObjectsGraphs.Add(soc);
        }

        sceneGameObjects = new Dictionary<string, SceneGameObject>();

        OnSessionLoaded.Invoke();

        showingGraph = sceneGraph;
    }

    public void AddSceneGameObject(SceneGameObject so){
        sceneGameObjects.Add(so.NTKey, so);
        OnSceneGameObjectsChanged.Invoke();
    }

    public void RemoveSceneGameObject(string key, Type variableType){

        if(sceneGameObjects.ContainsKey(key)){
            SceneGameObject toRemove = sceneGameObjects[key];
            sceneGameObjects.Remove(key);

            Destroy(toRemove.gameObject);
            _sceneVariables.variableRepository.RemoveVariable(key, variableType);
            OnSceneGameObjectsChanged.Invoke();

            for(int i = 0; i < sceneObjectsGraphs.Count; i++){
                if(sceneObjectsGraphs[i].linkedNTVariable == key){
                    sceneObjectsGraphs.RemoveAt(i);
                    OnGraphListChanged.Invoke();
                    showingGraph = sceneGraph;
                    return;
                }
            }  
        }
    }

    public void SetSelected(string key){

        if(selectedSceneObject != null) selectedSceneObject.isSelected = false;

        if(!string.IsNullOrEmpty(key) && sceneGameObjects.ContainsKey(key)){
            selectedSceneObject = sceneGameObjects[key];
            selectedSceneObject.isSelected = true;

            for(int i = 0; i < sceneObjectsGraphs.Count; i++){
                if(sceneObjectsGraphs[i].linkedNTVariable == key){
                    showingGraph = sceneObjectsGraphs[i];
                }
            }  
        }
        else
        {
            selectedSceneObject = null;
        }
    }

    public SceneGameObject GetSceneGameObject(string key){
        if(sceneGameObjects.ContainsKey(key)){
            return sceneGameObjects[key];
        }
        else
        {
            return null;
        }
    }

    public void OpenGraphFor(string key){
        for(int i = 0; i < sceneObjectsGraphs.Count; i++){
            if(sceneObjectsGraphs[i].linkedNTVariable == key){
                showingGraph = sceneObjectsGraphs[i];
                SetSelected(key);
                return; 
            }
        }

        SceneObjectGraph soc = ScriptableObject.CreateInstance<SceneObjectGraph>();
        soc.linkedNTVariable = key;
        soc.sceneVariables = sceneVariables;

        sceneObjectsGraphs.Add(soc);
        OnGraphListChanged.Invoke();

        showingGraph = soc;

        SetSelected(key);
    }

    public void OpenSceneGraph(){
        showingGraph = sceneGraph; 
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