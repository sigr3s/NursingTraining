using System;
using System.Collections.Generic;
using cakeslice;
using NT;
using NT.Graph;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;

[System.Serializable]
public struct SceneGameObjectData{
    //Scene data
    public string id;
    public List<string> childs;
    public string parent;
    public NTVariable data;
    public string sceneObjectGUID;

    //Graph
    public string graphJSON;

    //Transform
    public Vector3 position;
    public Vector3 rotation;
    public SceneObjectGraph graph;
}

public class SceneGameObject : MonoBehaviour, ISerializationCallbackReceiver
{

#region  Serialized
    [SerializeField] public SceneGameObjectData data;
#endregion



    [NonSerialized] private bool _isColliding = false;
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

    [NonSerialized] private bool _isMouseOver = false;
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

    [NonSerialized] private bool _isSelected = false;
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

    [NonSerialized] public bool isPlacingMode = false;
    [NonSerialized] public bool deleteMode = false;


    [NonSerialized] private SceneGameObject _parent;
    public SceneGameObject parent{
        get{
            if(_parent == null){
                _parent = transform.parent.GetComponent<SceneGameObject>();
            }
            return _parent;
        }

        set{
            _parent = value;
        }
    }

    [NonSerialized] private ISceneObject _sceneObject;
    public ISceneObject sceneObject {
        get{
            if(_sceneObject == null && !string.IsNullOrEmpty(data.sceneObjectGUID) ){
                _sceneObject =  SessionManager.Instance.sceneObjects.GetObject(data.sceneObjectGUID);
            }
            return _sceneObject;
        }
        set{
            _sceneObject = value;
            data.sceneObjectGUID = _sceneObject.GetGUID();
        }
    }


    [NonSerialized] private List<Outline> renderersOutlines;

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

    public void OnBeforeSerialize()
    {
        if(this == null){
        }
        
        if(this != null && transform != null){
            data.position = transform.localPosition;
            data.rotation = transform.localRotation.eulerAngles;
        }
    }

    public void OnAfterDeserialize()
    {
        if(data.childs == null){
            data.childs = new List<string>();
        }
    }

    public void LoadFromData(SceneGameObjectData data){
        this.data = data;
        RestoreTransform();
    }

    public void RestoreTransform(){
        transform.localPosition = data.position;
        transform.localRotation = Quaternion.Euler(data.rotation);
        transform.localScale = Vector3.one;
    }

    public virtual bool CanHoldItem(SceneGameObject previewSceneGameObject)
    {
        return sceneObject.CanHoldItem(previewSceneGameObject);
    }
}