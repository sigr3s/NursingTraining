using System;
using System.Collections;
using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditor : MonoBehaviour{   
    public enum MapMode{         
        Build,
        Edit,
        Inspect,
        Delete
    }

    [Header("References")]
    public Camera raycastCamera;
    public Toggle BuildToggle;
    public Toggle InspectToggle;
    public Toggle DeleteToggle;
    public MapRaycast mapRaycast;
    public Transform objectList;
    public Transform mapPivot;



    [Header("Map settings")]
    public float gridSize = 0.1f;
    public int placementLayer = 11;


    [Header("Map Object list")]
    public SceneObjects sceneObjects;
    public GameObject sceneObjectUiPrefab;
    
    [Header("Debug")]
    [Space(50)]
    public GameObject previewGO = null;
    public SceneObjectCollider previewGOSC = null;

    private ISceneObject current;
    private SceneObjectCollider currentSO;
    private SceneObjectCollider selectedSO;


    
    private MapMode _mode = MapMode.Inspect;

    public MapMode mode{
        get{
            return _mode;
        }
        set{
            _mode = value;

            switch(_mode){
                case MapMode.Build:
                    BuildToggle.isOn = true;
                break;
                case MapMode.Inspect:
                    InspectToggle.isOn = true;
                break;
                case MapMode.Delete:
                    DeleteToggle.isOn = true;
                break;
            }

            ResetCurrent(true, false);
        }
    }   
    private GameObject items;
    private LayerMask currentObjectLayer;
    public LayerMask allExceptFloor = ~0;

     private void Awake() {
        foreach (var so in sceneObjects.objectSet)
        {
            ISceneObject isc = (ISceneObject) so;

            if(isc == null) continue;

            UISceneObject uisc = isc.GetUISceneObject();
            
            GameObject soButton = Instantiate(sceneObjectUiPrefab, objectList);            
            Button button =  soButton.GetComponent<Button>();

            button.onClick.AddListener(() =>{ SetPlacementObject(isc); });

            button.targetGraphic.color = uisc.color;
            soButton.transform.Find("Icon").GetComponent<Image>().sprite = uisc.icon;

        }
    }

    void Start()
    {
        BuildToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Build;});
        InspectToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Inspect;});
        DeleteToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Delete;});

        mode = MapMode.Build;

        SessionManager.Instance.OnSessionLoaded.AddListener(LoadMap);

        LoadMap();
    }

    private void LoadMap(){
        if(items != null){
            Destroy(items);
        }

        items = new GameObject();
        items.transform.parent = mapPivot;
        items.transform.localPosition = Vector3.zero;
        items.name = "Items Container"; 

        NTVariableRepository repo = SessionManager.Instance.sceneVariables.variableRepository;

        foreach (var kvp in repo.dictionary)
        {
            string variable = kvp.Key;
            string displayName = variable.Replace("NT.Variables.NT", "");
            NTVariableDictionary varDict = kvp.Value;

            if(!typeof(INTSceneObject).IsAssignableFrom(varDict._dictType)){
                continue;
            }

            foreach (var kvpi in varDict)
            {
                INTSceneObject intSO =  (INTSceneObject) kvpi.Value;

                SceneObjectExtraData sed =  intSO.GetExtraData();
                SceneObject so = sceneObjects.GetObject(sed.sceneObjectGUID);

                if(so != null){
                    GameObject previewGO = Instantiate(so.GetModel(), items.transform);
            
                    SceneObjectCollider previewGOSC = previewGO.GetComponent<SceneObjectCollider>();

                    if(previewGOSC == null){
                        previewGOSC = previewGO.AddComponent<SceneObjectCollider>();
                    }

                    previewGOSC.NTKey = kvpi.Key;
                    previewGOSC.NTDataType = varDict._dictType;

                }
            }

        }

    }

    private void Update() {
    
        if(Input.GetKeyDown(KeyCode.Alpha1)){ mode = MapMode.Build; }
        if(Input.GetKeyDown(KeyCode.Alpha2)){ mode = MapMode.Inspect; }

        if(!mapRaycast.shouldRaycastMap){
            return;
        }

        switch(mode){
            case MapMode.Build:
                PlaceObject();
            break;
            case MapMode.Inspect:
                InspectObject();
            break;
        }
    }

    private bool TryRaycastFromScreen(LayerMask mask, out RaycastHit hit){
        Ray ray = raycastCamera.ViewportPointToRay(mapRaycast.textureCoords);

        if (Physics.Raycast(ray, out hit, 5000, mask)) {
            Transform objectHit = hit.transform;
            if(objectHit != null){
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            hit = new RaycastHit();
            return false;
        }
    }

    private void ResetCurrent(bool destroyPreview = false, bool cleanCurrent = true){
        if(cleanCurrent) current = null;

        if(currentSO){
            currentSO.isMouseOver = false;
        }

        if(destroyPreview) Destroy(previewGO);

        previewGO = null;
    }
    
    private void PlaceObject(){
        if(current == null) return;

        if(previewGO == null){
            previewGO = Instantiate(current.GetModel());
            
            previewGOSC = previewGO.GetComponent<SceneObjectCollider>();

            if(previewGOSC == null){
                previewGOSC = previewGO.AddComponent<SceneObjectCollider>();
            }

            previewGOSC.isPlacingMode = true;

            currentObjectLayer = previewGO.layer;
            previewGO.RunOnChildrenRecursive( (GameObject g) => {g.layer = placementLayer;} );
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            previewGO.transform.Rotate(new Vector3(0,1,0), 90);
        }

        if(Input.GetKeyDown(KeyCode.E)){
            previewGO.transform.Rotate(new Vector3(0,1,0), -90);
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            ResetCurrent(true);
            return;
        }


        if(TryRaycastFromScreen(current.GetLayerMask(), out RaycastHit hit)){
            Transform objectHit = hit.transform;
            Vector2 hitPointOnPlane = new Vector2(hit.point.x, hit.point.z);
            float x = hitPointOnPlane.x - hitPointOnPlane.x%gridSize;  
            float z = hitPointOnPlane.y - hitPointOnPlane.y%gridSize; 

            previewGO.transform.position = new Vector3(x, hit.point.y, z);

            if(Input.GetMouseButtonDown(0) && !previewGOSC.isColliding){

                previewGO.transform.parent = items.transform;
                previewGO.RunOnChildrenRecursive( (GameObject g) => {g.layer = currentObjectLayer;} );
                previewGOSC.isPlacingMode = false;
                Type t = current.GetDataType();
                string key = current.GetName() + previewGO.GetHashCode();

                INTSceneObject savedSceneObject = (INTSceneObject) Activator.CreateInstance(t);
                savedSceneObject.SetName(key);
                savedSceneObject.SetScriptableObject(current.GetGUID());

                savedSceneObject.SetPosition(previewGOSC.transform.localPosition);
                savedSceneObject.SetRotation(previewGOSC.transform.localRotation.eulerAngles);
                

                SessionManager.Instance.sceneVariables.variableRepository.AddVariable(t, savedSceneObject);

                previewGOSC.NTDataType = t;
                previewGOSC.NTKey = key;

                previewGO = null;
            }
        }
    }

    private void InspectObject(){
        if(TryRaycastFromScreen(allExceptFloor, out RaycastHit hit)){
            SceneObjectCollider soc = hit.transform.GetComponent<SceneObjectCollider>();

            if(soc != null){
                if(currentSO != null){
                    currentSO.isMouseOver = false;
                }

                currentSO = soc;
                currentSO.isMouseOver = true;

                if( Input.GetMouseButtonDown(0) ){
                    if(selectedSO != null) selectedSO.isSelected = false;

                    currentSO.isSelected = true;
                    selectedSO = currentSO;
                    SessionManager.Instance.selectedSceneObject = selectedSO;
                }
            }
            else if(currentSO != null){
                currentSO.isMouseOver = false;
            }
        }
        else if(currentSO != null){
            currentSO.isMouseOver = false;
        }
    }
    
    public void SetPlacementObject(ISceneObject sceneObject){
        ResetCurrent(true);
        current = sceneObject;
    }
}
