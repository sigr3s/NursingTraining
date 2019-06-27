using UnityEngine;
using VRTK;
using NT;
using System;

public class ToolSceneGameObject : SceneGameObject, ITool
{
    public Tools toolType;
    public VRTK_InteractGrab grabObject;

    private void Start()
    {
        if (grabObject != null)
        {
            grabObject.ControllerGrabInteractableObject += ObjectGrab;
        }
    }

    private void ObjectGrab(object sender, ObjectInteractEventArgs e)
    {
        Debug.Log("Object Grab");
    }

    public Tools GetToolType()
    {
        return toolType;
    }
}