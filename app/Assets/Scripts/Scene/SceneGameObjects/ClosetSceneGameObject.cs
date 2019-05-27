using System;
using System.Collections.Generic;
using NT.SceneObjects;
using UnityEngine;
using UnityEngine.Events;

public class ClosetSceneGameObject : SceneGameObject {
    public GameObject drawer;
    public List<Transform> slotPivot;

    public override bool CanHoldItem(SceneGameObject previewSceneGameObject){
        ClosetData closetData = (ClosetData) data.data.GetValue();

        List<SceneGameObjectReference> slots = SlotsList(closetData);

        bool freeSpace = false;

        foreach (var s in slots)
        {
            freeSpace = freeSpace || s == null;
        }

        return freeSpace;
    }

    public override void HoldItem(SceneGameObject childOfElement, bool instancing = true){
        ClosetData closetData = (ClosetData) data.data.GetDefaultValue();
        List<SceneGameObjectReference> slots = SlotsList(closetData);


        if(instancing){
            
            bool stored = false;

            for (int i = 0; i < slots.Count && !stored; i++)
            {
                if(slots[i] == null){
                    stored = true;
                    slots[i] = new SceneGameObjectReference(childOfElement);
                }
            }

            ListToSlots(ref closetData, slots);

            data.data.SetDefaultValue(closetData);
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if(slots[i].linkedSGO == childOfElement.data.id){
                    
                    childOfElement.transform.SetParent(slotPivot[i]);
                    childOfElement.transform.localPosition = Vector3.zero;
                    childOfElement.transform.localRotation = Quaternion.identity;

                    
                    return;
                }
            }

            Debug.Log("WTF¿");
            childOfElement.gameObject.SetActive(false);            
        }
    }

    public override void LoadFromData(SceneGameObjectData data){
        base.LoadFromData(data);
    }

    private List<SceneGameObjectReference> SlotsList(ClosetData closetData){
        return new List<SceneGameObjectReference>() { closetData.mainDrawer.slot00,closetData.mainDrawer.slot01,closetData.mainDrawer.slot02, closetData.mainDrawer.slot03, closetData.mainDrawer.slot04,closetData.mainDrawer.slot05 };
    }

    private void ListToSlots(ref ClosetData closetData, List<SceneGameObjectReference> dataObjects){
        closetData.mainDrawer.slot00 = dataObjects[0];
        closetData.mainDrawer.slot01 = dataObjects[1];
        closetData.mainDrawer.slot02 = dataObjects[2];
        closetData.mainDrawer.slot03 = dataObjects[3];
        closetData.mainDrawer.slot04 = dataObjects[4];
        closetData.mainDrawer.slot05 = dataObjects[5];
    }

    public override void RestoreTransform(){
        base.RestoreTransform();

        //drawer.SetActive(false);

        //drawer.SetActive(true);
    }
}