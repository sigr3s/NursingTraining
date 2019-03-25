﻿using System;
using System.Collections;
using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour{   
    public enum MapMode{
        Placing,
        Edit,
        Inspect,
        Remove
    }

    [Header("References")]
    public Camera raycastCamera;
    public RuntimeInspector runtimeInspector;
    public SceneVariables sceneVariables;


    [Header("Map settings")]
    public float gridSize = 0.1f;
    public int selectedObject  = 0;
    public int placementLayer = 11;

    [Space]
    [Header("Debug")]
    public ISceneObject current;
    public GameObject previewGO = null;
    public SceneObjectCollider previewGOSC = null;
    public MapMode mode = MapMode.Inspect;
    

    
    private GameObject items;
    private LayerMask currentObjectLayer;
    public LayerMask allExceptFloor = ~0;

    void Start()
    {
        items = new GameObject();
        items.transform.parent = this.transform;
        items.transform.localPosition = Vector3.zero;
        items.name = "Items Container"; 
    }

    private void Update() {
        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(Input.GetKeyDown(KeyCode.Alpha1)){ mode = MapMode.Placing; ResetCurrent(true, false); }
        if(Input.GetKeyDown(KeyCode.Alpha2)){ mode = MapMode.Inspect; ResetCurrent(true, false); }

        switch(mode){
            case MapMode.Placing:
                PlaceObject();
            break;
            case MapMode.Inspect:
                InspectObject();
            break;
        }
    }

    private bool TryRaycastFromScreen(LayerMask mask, out RaycastHit hit){
        Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);

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