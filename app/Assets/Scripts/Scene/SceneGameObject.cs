using System;
using System.Collections.Generic;
using cakeslice;
using NT;
using NT.SceneObjects;
using UnityEngine;

public class SceneGameObject : MonoBehaviour
{
    private bool _isColliding = false;
    public bool isColliding{
        get{
            return _isColliding;
        }
        set{
            _isColliding = value;

            if(_isColliding){
                foreach (var rendererOutline in renderersOutlines)
                {
                    rendererOutline.enabled = true;
                    rendererOutline.color = 2;
                }
            }
            else
            {
                foreach (var rendererOutline in renderersOutlines)
                {
                    rendererOutline.enabled = false;
                }
            }
        }
    }

    private bool _isMouseOver = false;
    public bool isMouseOver{
        get{
            return _isMouseOver;
        }
        set{
            _isMouseOver = value;

            if( (_isMouseOver && !_isSelected) || (_isMouseOver && deleteMode) ){
                foreach (var rendererOutline in renderersOutlines)
                {
                    rendererOutline.enabled = true;                    
                    rendererOutline.color = deleteMode ? 2 : 1;
                }
            }
            else if(!_isSelected)
            {
                foreach (var rendererOutline in renderersOutlines)
                {
                    rendererOutline.enabled = false;
                }
            }
        }
    }

    private bool _isSelected = false;
    public bool isSelected{
        get{
            return _isSelected;
        }
        set{
            _isSelected = value;

            if(_isSelected){
                foreach (var rendererOutline in renderersOutlines)
                {
                    rendererOutline.enabled = true;
                    rendererOutline.color = 0;
                }
            }
            else if(!isMouseOver)
            {
                foreach (var rendererOutline in renderersOutlines)
                {
                    rendererOutline.enabled = false;
                }
            }
        }
    }
    
    public bool isPlacingMode = false;
    public bool deleteMode = false;

    public string NTKey;
    public Type NTDataType;
    public ISceneObject sceneObject;

    public List<Outline> renderersOutlines;

    private void Awake() {
        List<Renderer> renderers = new List<Renderer>( GetComponentsInChildren<Renderer>() );
        renderersOutlines = new List<Outline>();

        foreach (var r in renderers)
        {
            Outline rendererOutiline =  r.gameObject.AddComponent<Outline>();
            rendererOutiline.enabled = false;
            renderersOutlines.Add(rendererOutiline);           
        }
    }


    private void OnTriggerEnter(Collider other) {
        if(!isPlacingMode) return;

        isColliding = true;
    }

    private void OnTriggerExit(Collider other) {
        if(!isPlacingMode) return;
        
        isColliding = false;
    }

}