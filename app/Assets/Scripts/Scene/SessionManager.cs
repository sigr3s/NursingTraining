using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NT;
using NT.Graph;
using NT.SceneObjects;
using NT.Variables;

using OdinSerializer;
using SimpleJSON;

using UnityEngine;
using UnityEngine.Events;
using XNode;

public class SessionManager : Singleton<SessionManager>, IVariableDelegate {

    [Header("Session Data")]
    public SessionData SessionData;
    public bool loadOnAwake = true;

    //public static string exportPath = Application.dataPath + "/Saves/Sessions/";


    [Header("References")]
    public SceneGraph sceneGraph;
    public SceneObjects sceneObjects;
    public IMapLoader mapLoader;


    [Space(20)]
    [Header("Debug")]
    public Dictionary<string, SceneGameObject> sceneGameObjects;
    public Dictionary<string, object> userVariables = new Dictionary<string, object>();
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

    private NodeGraph _showingGraph;
    public NodeGraph showingGraph{
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
            sceneGraph = new SceneGraph();
        }

        sceneGameObjects = new Dictionary<string, SceneGameObject>();

        sceneObjects.LoadPrefabs();

        var sceneRoot = new GameObject("SceneRoot");
    }

    private void Start() {
        if(loadOnAwake){
           LoadSession(SessionData.sessionID);
        }
    }


    [ContextMenu("Start execution")]
    public void StartExecution(){

        //Reset all variables to default values!
        foreach(var so in sceneGameObjects){
            SceneGameObject scgo = so.Value;

            scgo.data.data.Reset();

            if(scgo.data.graph != null && (scgo.data.graph.nodes.Count > 0 || scgo.data.graph.packedNodes.Count > 0) ){
                scgo.data.graph.StartExecution();
            }
        }

        sceneGraph.StartExecution();


        MessageSystem.SendMessage("Application Start");
    }

#region Export/Import

    [ContextMenu("User variables")]
    public void UserVariablesTest(){
        var d = new Dictionary<string, object>();

        d.Add("myString", "hey");
        d.Add("some", 1);
        d.Add("test", false);

        byte[] ojson = SerializationUtility.SerializeValue(d, DataFormat.JSON);
        var d1 = SerializationUtility.DeserializeValue<Dictionary<string, object>>(ojson, DataFormat.JSON);


        foreach(var it in d1){
            Debug.Log(it.Value.GetType().IsString());
            Debug.Log(it.Value.GetType().IsBool());
            Debug.Log(it.Value.GetType().IsNumber());
            Debug.Log(it.Key + " ___ " + it.Value);
        }

    }

    public void SaveScene(string path){

        byte[] ojson = SerializationUtility.SerializeValue(sceneGameObjects, DataFormat.JSON);

        if(!string.IsNullOrEmpty(path)){
            File.WriteAllBytes(path, ojson);
        }
    }

    [ContextMenu("Save")]
    public void SaveSession(){
        if(string.IsNullOrEmpty(SessionData.sessionID)){
            SessionData.sessionID = DateTime.Now.ToString();
        }

        Debug.Log( $" Saving session to: {Application.persistentDataPath} ");

        string saveFolder = Application.dataPath + "/Saves/Sessions/" + SessionData.sessionID;
        if(Directory.Exists(saveFolder) ){
            Directory.Delete(saveFolder, true);
        }

        Directory.CreateDirectory(saveFolder);


        SessionData.lastModified = DateTime.Now.ToString();

        SessionData.sceneGraphFile = "sceneGraph.nt";

        byte[] sceneGraphData = SerializationUtility.SerializeValue(sceneGraph, DataFormat.JSON);

        File.WriteAllBytes(saveFolder + "/" + SessionData.sceneGraphFile, sceneGraphData);

        SessionData.sceneFile = "scene.nt";
        SaveScene(saveFolder + "/" + SessionData.sceneFile);

        string sessionJSON = JsonUtility.ToJson(SessionData);
        File.WriteAllText(saveFolder + "/" + "config.nt", sessionJSON);

    }

    public void LoadScene(string path){
        byte[] ojson = File.ReadAllBytes(path);

        var newRoot = SerializationUtility.DeserializeValue<Dictionary<string,SceneGameObject>>(ojson, DataFormat.JSON);

        mapLoader.LoadMap(newRoot);
    }

    public void LoadSession(string sessionID){

        string saveFolder = Application.dataPath + "/Saves/Sessions/" + sessionID;

        if(string.IsNullOrEmpty(sessionID)) return;

        if(!Directory.Exists(saveFolder) ){
            return;
        }

        sceneGameObjects = new Dictionary<string, SceneGameObject>();


        string configJSON = File.ReadAllText(saveFolder + "/" + "config.nt");
        SessionData = JsonUtility.FromJson<SessionData>(configJSON);

        byte[] sceneGraphData = File.ReadAllBytes(saveFolder +  "/" + SessionData.sceneGraphFile);
        sceneGraph = SerializationUtility.DeserializeValue<SceneGraph>(sceneGraphData, DataFormat.JSON);
        sceneGraph.variableDelegate = this;

        LoadScene(saveFolder +  "/" + SessionData.sceneFile);

        OnSessionLoaded.Invoke();

        showingGraph = sceneGraph;
    }
#endregion

#region Scene GameObjcts
    public void AddSceneGameObject(SceneGameObject so){
        sceneGameObjects.Add(so.data.id, so);

        if(so.data.graph != null){
            so.data.graph.variableDelegate = this;
        }

        OnSceneGameObjectsChanged.Invoke();
        OnGraphListChanged.Invoke();
    }

    public void RemoveSceneGameObject(string key){

        if(sceneGameObjects.ContainsKey(key)){
            SceneGameObject toRemove = sceneGameObjects[key];
            sceneGameObjects.Remove(key);

            if(toRemove.data.graph != null){
                showingGraph = sceneGraph;
            }

            if(!string.IsNullOrEmpty(toRemove.data.parent) ){
                SceneGameObject toRemoveParent = sceneGameObjects[toRemove.data.parent];
                toRemoveParent.data.childs.Remove(key);

                Debug.Log("Removed from parent!");
            }

            List<SceneGameObject> childsToRemove = new List<SceneGameObject>(toRemove.GetComponentsInChildren<SceneGameObject>());

            foreach(SceneGameObject childToRemove in childsToRemove){
                string childKey = childToRemove.data.id;
                sceneGameObjects.Remove(childKey);
            }

            Destroy(toRemove.gameObject);
            OnSceneGameObjectsChanged.Invoke();
            OnGraphListChanged.Invoke();
        }
    }

    public void SetSelected(string key){

        if(selectedSceneObject != null) selectedSceneObject.isSelected = false;

        if(!string.IsNullOrEmpty(key) && sceneGameObjects.ContainsKey(key)){
            selectedSceneObject = sceneGameObjects[key];
            selectedSceneObject.isSelected = true;

            if(selectedSceneObject.data.graph != null){
                showingGraph = selectedSceneObject.data.graph;
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

        if(sobj.data.graph == null){
            SceneObjectGraph soc = new SceneObjectGraph();
            soc.linkedNTVariable = sobj.data.id;
            soc.variableDelegate = this;

            sobj.data.graph = soc;
        }

        showingGraph = sobj.data.graph;
        SetSelected(key);

        OnGraphListChanged.Invoke();

    }

    public void OpenSceneGraph(){
        showingGraph = sceneGraph;
    }

    public List<SceneObjectGraph> GetAllGraphs()
    {
        List<SceneObjectGraph> graphs =  new List<SceneObjectGraph>();

        if(sceneGameObjects == null) return graphs;

        foreach(var sceneGameObject in sceneGameObjects){
            if( sceneGameObject.Value.data.graph != null &&
                (sceneGameObject.Value.data.graph.nodes.Count > 0 || sceneGameObject.Value.data.graph == showingGraph)){
                SceneObjectGraph sog = sceneGameObject.Value.data.graph;

                sog.linkedNTVariable = sceneGameObject.Value.data.id;
                sog.displayName = sceneGameObject.Value.name;

                if(sog.variableDelegate == null) sog.variableDelegate = this;

                graphs.Add(sceneGameObject.Value.data.graph);
            }
        }

        return graphs;
    }

    public object GetValue(string key)
    {
        SceneGameObject scgo =  GetSceneGameObject(key);
        if(scgo != null){
            return scgo.data.data.GetValue();
        }

        return null;
    }

    public void SetValue(string key, object value){
        SceneGameObject scgo =  GetSceneGameObject(key);
        if(scgo != null){
            scgo.data.data.SetValue(value);
        }
    }

    public object GetUserVariable(string key)
    {
        throw new NotImplementedException();
    }

    public object SetUserVariable(string key)
    {
        throw new NotImplementedException();
    }


    #endregion
}

[System.Serializable]
public struct SessionData{
    public string sessionID;
    public string lastModified;
    public string sceneGraphFile;
    public string sceneFile;
}