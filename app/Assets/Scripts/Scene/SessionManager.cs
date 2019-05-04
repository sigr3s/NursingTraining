using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NT.Graph;
using NT.SceneObjects;
using NT.Variables;
using OdinSerializer;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class SessionManager : Singleton<SessionManager> {

    [Header("Session Data")]
    public SessionData SessionData;
    public bool loadOnAwake = true;


    [Header("References")]
    public SceneGraph sceneGraph;
    public SceneObjects sceneObjects;
    public IMapLoader mapLoader;


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

        sceneGameObjects = new Dictionary<string, SceneGameObject>();

        sceneObjects.LoadPrefabs();

        var sceneRoot = new GameObject("SceneRoot");


        if(loadOnAwake){
           LoadSession(SessionData.sessionID);
        }
    }

#region Export/Import
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

        string saveFolder = Application.dataPath + "/Saves/" + SessionData.sessionID;
        if(Directory.Exists(saveFolder) ){
            Directory.Delete(saveFolder, true);
        }

        Directory.CreateDirectory(saveFolder);


        SessionData.lastModified = DateTime.Now.ToString();

        SessionData.sceneGraphFile = "sceneGraph.json";
        sceneGraph.Export(saveFolder + "/" + SessionData.sceneGraphFile);

        SessionData.sceneFile = "scene.json";
        SaveScene(saveFolder + "/" + SessionData.sceneFile);

        string sessionJSON = JsonUtility.ToJson(SessionData);
        File.WriteAllText(saveFolder + "/" + "config.json", sessionJSON);

    }

    public void LoadScene(){
        //FIXME: 
        byte[] ojson = SerializationUtility.SerializeValue(sceneGameObjects, DataFormat.JSON);

        var newRoot = SerializationUtility.DeserializeValue<Dictionary<string,SceneGameObject>>(ojson, DataFormat.JSON);

        foreach(var c in newRoot){
            GameObject go = new GameObject(c.Key);
            SceneGameObject scgo = go.AddComponent<SceneGameObject>();
            scgo.LoadFromData(c.Value.data);
        }
    }

    public void LoadSession(string sessionID){

        string saveFolder = Application.dataPath + "/Saves/" + sessionID;

        if(string.IsNullOrEmpty(sessionID)) return;

        if(!Directory.Exists(saveFolder) ){
            return;
        }


        string configJSON = File.ReadAllText(saveFolder + "/" + "config.json");
        SessionData = JsonUtility.FromJson<SessionData>(configJSON);


        sceneGraph.Import(saveFolder +  "/" + SessionData.sceneGraphFile);


        sceneGameObjects = new Dictionary<string, SceneGameObject>();

        OnSessionLoaded.Invoke();

        showingGraph = sceneGraph;
    }
#endregion

#region Scene GameObjcts
    public void AddSceneGameObject(SceneGameObject so){
        sceneGameObjects.Add(so.data.id, so);
        OnSceneGameObjectsChanged.Invoke();
    }

    public void RemoveSceneGameObject(string key){

        if(sceneGameObjects.ContainsKey(key)){
            SceneGameObject toRemove = sceneGameObjects[key];
            sceneGameObjects.Remove(key);

            if(toRemove.graph != null){
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
    public string sceneGraphFile;
    public string sceneFile;
}