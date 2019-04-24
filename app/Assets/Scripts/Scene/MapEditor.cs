using System;
using System.Collections;
using System.Collections.Generic;
using NT;
using NT.Graph;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditor : MapLoader {   
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


    [Header("Map settings")]
    public float gridSize = 0.1f;
    public int placementLayer = 11;


    [Header("Map Object list")]
    public GameObject sceneObjectUiPrefab;
    public Sprite prefabSprite;

    [Header("Debug")]
    [Space(50)]
    public GameObject previewGO = null;
    public SceneGameObject previewSceneGameObject = null;

    private ISceneObject current;
    private SceneGameObject currentSceneGameObject;

    private Vector3 lastRotation = Vector3.zero;

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
    private LayerMask currentObjectLayer;
    public LayerMask allExceptFloor = ~0;

     private void LoadObjectsButtons() {
        foreach (var so in SessionManager.Instance.sceneObjects.objectSet)
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

        foreach (var so in SessionManager.Instance.sceneObjects.prefabSet)
        {
            ISceneObject isc = (ISceneObject) so;

            if(isc == null) continue;

            UISceneObject uisc = isc.GetUISceneObject();

            GameObject soButton = Instantiate(sceneObjectUiPrefab, objectList);
            Button button =  soButton.GetComponent<Button>();

            button.onClick.AddListener(() =>{ SetPlacementObject(isc); });

            button.targetGraphic.color = Color.yellow;
            soButton.transform.Find("Icon").GetComponent<Image>().sprite = prefabSprite;

        }
    }

    void Start()
    {
        SessionManager.Instance.OnSessionLoaded.AddListener(LoadMap);

        BuildToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Build;});
        InspectToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Inspect;});
        DeleteToggle.onValueChanged.AddListener( (bool active) => { if(active) mode = MapMode.Delete;});

        mode = MapMode.Build;

        LoadObjectsButtons();
        LoadMap();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1)){ mode = MapMode.Build; }
        if(Input.GetKeyDown(KeyCode.Alpha2)){ mode = MapMode.Inspect; }
        if(Input.GetKeyDown(KeyCode.Alpha3)){ mode = MapMode.Delete; }


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
            case MapMode.Delete:
                DeleteObjects();
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

        if(currentSceneGameObject){
            currentSceneGameObject.isMouseOver = false;
            currentSceneGameObject.deleteMode = false;
        }

        if(destroyPreview) Destroy(previewGO);

        previewGO = null;
    }
    
    private void PlaceObject(){
        if(current == null) return;

        if(previewGO == null){
            previewGO = current.GetPreviewGameObject();

            previewGO.transform.localRotation = Quaternion.Euler(lastRotation);
            
            previewSceneGameObject = previewGO.GetComponent<SceneGameObject>();

            if(previewSceneGameObject == null){
                previewSceneGameObject = previewGO.AddComponent<SceneGameObject>();
            }

            previewSceneGameObject.isPlacingMode = true;

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
            Vector3 hitPointOnPlane = hit.point;
            float x = hitPointOnPlane.x - hitPointOnPlane.x%gridSize;  
            float z = hitPointOnPlane.z - hitPointOnPlane.z%gridSize;
            float y = hitPointOnPlane.y - hitPointOnPlane.y%gridSize;

            previewGO.transform.position = hitPointOnPlane;

            SceneGameObject sco = objectHit.GetComponentInParent<SceneGameObject>();

            if(sco != null){
                if(sco.sceneObject.CanHoldItem(previewSceneGameObject)){
                    previewGO.SetActive(false);
                    SessionManager.Instance.SetSelected(sco.NTKey);
                }
            }
            else
            {
                previewGO.SetActive(true);
                SessionManager.Instance.SetSelected(null);
            }

            if(Input.GetMouseButtonDown(0) && (!previewSceneGameObject.isColliding || sco != null) ){
                Transform p = sco != null ? sco.transform : items.transform;
                previewGO.transform.parent = p;

                previewGO.RunOnChildrenRecursive( (GameObject g) => {g.layer = currentObjectLayer;} );
                previewSceneGameObject.isPlacingMode = false;
                lastRotation = previewSceneGameObject.transform.localRotation.eulerAngles;
                
                SceneGameObject instanced = current.Instantiate(SessionManager.Instance.sceneVariables.variableRepository, p,
                                    previewGO.transform.localPosition, previewGO.transform.localRotation);
    
                SessionManager.Instance.AddSceneGameObject(instanced);

                if(sco != null){
                    sco.sceneObject.HoldItem( instanced , sco);
                }

                Destroy(previewGO);

                previewGO = null;
            }
        }
    }

    private void DeleteObjects()
    {
        if(TryRaycastFromScreen(allExceptFloor, out RaycastHit hit)){
            SceneGameObject soc = hit.transform.GetComponent<SceneGameObject>();

            if(soc != null){
                if(currentSceneGameObject != null){
                    currentSceneGameObject.isMouseOver = false;
                    currentSceneGameObject.deleteMode = false;
                }

                currentSceneGameObject = soc;
                currentSceneGameObject.deleteMode = true;
                currentSceneGameObject.isMouseOver = true;
                
                if( Input.GetMouseButtonDown(0) ){
                    SessionManager.Instance.RemoveSceneGameObject(currentSceneGameObject.NTKey, currentSceneGameObject.NTDataType);
                }
            }
            else
            {
                if(currentSceneGameObject != null){                    
                    currentSceneGameObject.deleteMode = false;
                    currentSceneGameObject.isMouseOver = false;
                }

                currentSceneGameObject = null;
            }
        }
        else
        {
            if(currentSceneGameObject != null){                    
                currentSceneGameObject.deleteMode = false;
                currentSceneGameObject.isMouseOver = false;
            }

            currentSceneGameObject = null;
        }
    }


    private void InspectObject(){
        if(TryRaycastFromScreen(allExceptFloor, out RaycastHit hit)){
            SceneGameObject soc = hit.transform.GetComponent<SceneGameObject>();

            if(soc != null){
                if(currentSceneGameObject != null){
                    currentSceneGameObject.isMouseOver = false;
                }

                currentSceneGameObject = soc;
                currentSceneGameObject.isMouseOver = true;

                if( Input.GetMouseButtonDown(0) ){
                    SessionManager.Instance.SetSelected(currentSceneGameObject.NTKey);
                }
            }
            else if(currentSceneGameObject != null){
                currentSceneGameObject.isMouseOver = false;
            }
        }
        else if(currentSceneGameObject != null){
            currentSceneGameObject.isMouseOver = false;
        }
    }
    
    public void SetPlacementObject(ISceneObject sceneObject){
        ResetCurrent(true);
        current = sceneObject;
    }
}
