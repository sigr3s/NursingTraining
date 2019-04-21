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


    [Header("References")]
    public SceneGraph sceneGraph;
    public SceneObjects sceneObjects;
    public SavedScene loadedScene;


    [Space(20)]
    [Header("Debug")]

    public Dictionary<string, SceneGameObject> sceneGameObjects;
    private SceneGameObject _selectedObjectSceneObject;

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

    private NTGraph _showingGraph;


    public NTGraph showingGraph{
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

        sceneObjects.LoadPrefabs();


        if(loadOnAwake){
           LoadSession(SessionData.sessionID);
        }
    }

    [System.Serializable]
    public struct SavedScene{
        public List<SavedSceneObject> objects;
    }

    [System.Serializable]
    public struct SavedSceneObject{
        public string parent;
        public string ScriptableObjectGUID;
        public string AssignedNTVariable;
        public Vector3 position;
        public Vector3 rotation;
        public string serializedGraph;
    }

#region Export/Import
    public void SaveScene(string path){

        SavedScene savedScene = new SavedScene(){ objects = new List<SavedSceneObject>()};

        foreach(var sceneGameObject in sceneGameObjects){

            SavedSceneObject savedObject = new SavedSceneObject(){
                ScriptableObjectGUID = sceneGameObject.Value.sceneObject.GetGUID(),
                AssignedNTVariable = sceneGameObject.Key,
                position = sceneGameObject.Value.gameObject.transform.localPosition,
                rotation = sceneGameObject.Value.gameObject.transform.localRotation.eulerAngles,
                parent = sceneGameObject.Value.parent != null ? sceneGameObject.Value.parent.NTKey : ""
            };

            if(sceneGameObject.Value.graph != null){
                savedObject.serializedGraph = sceneGameObject.Value.graph.ExportSerialized();
            }

            savedScene.objects.Add(savedObject);
        }

        if(!string.IsNullOrEmpty(path)){
            File.WriteAllText(path, JsonUtility.ToJson(savedScene));
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

        SessionData.sceneFile = "scene.json";
        SaveScene(saveFolder + "/" + SessionData.sceneFile);

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

        string sceneJSON = File.ReadAllText(saveFolder +  "/" + SessionData.sceneFile);
        loadedScene = JsonUtility.FromJson<SavedScene>(sceneJSON);

        sceneGameObjects = new Dictionary<string, SceneGameObject>();

        OnSessionLoaded.Invoke();

        showingGraph = sceneGraph;
    }
#endregion

#region Scene GameObjcts
    public void AddSceneGameObject(SceneGameObject so){
        sceneGameObjects.Add(so.NTKey, so);
        OnSceneGameObjectsChanged.Invoke();
    }

    public void RemoveSceneGameObject(string key, Type variableType){

        if(sceneGameObjects.ContainsKey(key)){
            SceneGameObject toRemove = sceneGameObjects[key];
            sceneGameObjects.Remove(key);

            if(toRemove.graph != null){
                showingGraph = sceneGraph;
            }

            List<SceneGameObject> childsToRemove = new List<SceneGameObject>(toRemove.GetComponentsInChildren<SceneGameObject>());

            foreach(SceneGameObject childToRemove in childsToRemove){
                string childKey = childToRemove.NTKey;
                sceneGameObjects.Remove(childKey);
                 _sceneVariables.variableRepository.RemoveVariable(childKey, childToRemove.NTDataType);
            }

            Destroy(toRemove.gameObject);
            _sceneVariables.variableRepository.RemoveVariable(key, variableType);
            OnSceneGameObjectsChanged.Invoke();
            OnGraphListChanged.Invoke();
        }
    }

    public void SetSelected(string key){

        if(selectedSceneObject != null) selectedSceneObject.isSelected = false;

        if(!string.IsNullOrEmpty(key) && sceneGameObjects.ContainsKey(key)){
            selectedSceneObject = sceneGameObjects[key];
            selectedSceneObject.isSelected = true;

            if(selectedSceneObject.graph != null){
                showingGraph = selectedSceneObject.graph;
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
#endregion

#region  Graph functions
    public void OpenGraphFor(string key){
        SceneGameObject sobj = GetSceneGameObject(key);

        if(sobj == null) return;

        if(sobj.graph == null){
            SceneObjectGraph soc = ScriptableObject.CreateInstance<SceneObjectGraph>();
            soc.linkedNTVariable = key;
            soc.sceneVariables = sceneVariables;

            sobj.graph = soc;
            OnGraphListChanged.Invoke();
        }

        showingGraph = sobj.graph;
        SetSelected(key);
    }

    public void OpenSceneGraph(){
        showingGraph = sceneGraph;
    }

    public List<SceneObjectGraph> GetAllGraphs()
    {
        List<SceneObjectGraph> graphs =  new List<SceneObjectGraph>();

        foreach(var sceneGameObject in sceneGameObjects){
            if(sceneGameObject.Value.graph != null){
                graphs.Add(sceneGameObject.Value.graph);
            }
        }

        return graphs;
    }
#endregion
}

[System.Serializable]
public struct SessionData{
    public string sessionID;
    public string lastModified;
    public string variablesFile;
    public string sceneGraphFile;
    public string sceneFile;
}