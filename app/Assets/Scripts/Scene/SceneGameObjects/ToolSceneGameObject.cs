using UnityEngine;
using VRTK;
using NT;
using System;

public class ToolSceneGameObject : SceneGameObject, ITool
{
    public Tools toolType;
    public VRTK_InteractableObject grabObject;

    private void Start()
    {
        grabObject = GetComponentInChildren<VRTK_InteractableObject>();
        if (grabObject != null)
        {
            grabObject.InteractableObjectGrabbed += ObjectGrabbed;
        }
    }

    private void ObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        MessageSystem.SendMessage(data.id + "Grabbed");
    }


    public Tools GetToolType()
    {
        return toolType;
    }
}