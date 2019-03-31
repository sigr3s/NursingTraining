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
    public RuntimeInspector runtimeInspector;
    public SceneVariables sceneVariables;


    [Header("UI")]
    public Toggle BuildToggle;
    public Toggle InspectToggle;
    public Toggle DeleteToggle;

    public MapRaycast mapRaycast;


    [Header("Map settings")]
    public float gridSize = 0.1f;
    public int selectedObject  = 0;
    public int placementLayer = 11;

    [Space]
    [Header("Debug")]
    public ISceneObject current;
    public GameObject previewGO = null;
    public SceneObjectCollider previewGOSC = null;
    
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

    void Start()
    {
        items = new GameObject();
        items.transform.parent = this.transform;
        items.transform.localPosition = Vector3.zero;
        items.name = "Items Container"; 

        sceneVariables = SceneManager.Instance.sceneVariables;

        BuildToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Build;});
        InspectToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Inspect;});
        DeleteToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Delete;});

        mode = MapMode.Build;
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

        //Compute position from screen to RaycastMap

        //Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
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
        if(destroyPreview) Destroy(previewGO);
        if(cleanCurrent) current = null;

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

            previewGOSC.assignedSo = (ISceneObject) ScriptableObject.Instantiate(current.GetScriptableObject());
            previewGOSC.assignedSo.SetName(current.GetName() + " _ " + previewGO.GetInstanceID());
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

            if(Input.GetMouseButtonDown(0) && !previewGOSC.colliding){

                previewGO.transform.parent = items.transform;
                previewGO.RunOnChildrenRecursive( (GameObject g) => {g.layer = currentObjectLayer;} );
                previewGOSC.isPlacingMode = false;
                Type t = current.GetDataType();

                INTSceneObject savedSceneObject = (INTSceneObject) Activator.CreateInstance(t);
                savedSceneObject.SetName(previewGOSC.assignedSo.GetName());
                savedSceneObject.SetScriptableObject(previewGOSC.assignedSo.GetGUID());
                savedSceneObject.SetPosition(previewGOSC.transform.localPosition);
                savedSceneObject.SetRotation(previewGOSC.transform.localRotation.eulerAngles);
                

                sceneVariables.variableRepository.AddVariable(t, savedSceneObject);

                previewGO = null;
            }
        }
    }

    private void InspectObject(){
        if(TryRaycastFromScreen(allExceptFloor, out RaycastHit hit)){
            SceneObjectCollider soc = hit.transform.GetComponent<SceneObjectCollider>();

            if(soc != null){
                current = soc.assignedSo;

                runtimeInspector.SetMouseOver(current);

                if( Input.GetMouseButtonDown(0) ){
                    object value =  sceneVariables.variableRepository.GetValue(current.GetName(), current.GetDataType());
                    runtimeInspector.SetCurrent(current, value);
                }
            }
        }
    }
    
    public void SetPlacementObject(ISceneObject sceneObject){
        ResetCurrent(true);

        current = sceneObject;
    }
}
