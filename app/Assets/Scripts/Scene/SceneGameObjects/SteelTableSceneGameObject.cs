using NT;
using NT.SceneObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SteelTableSceneGameObject : SceneGameObject
{
    public GameObject coverVisual;
    public List<VRTK_SnapDropZone> dropZones;
    public bool anyItem = false;

    private void Start()
    {
        foreach(var dropZone in dropZones)
        {
            dropZone.ObjectSnappedToDropZone += OnObjectSnapped;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cover")
        {
            other.gameObject.SetActive(false);
            SetCover();
        }
    }

    public void OnObjectSnapped(object sender, SnapDropZoneEventArgs e)
    {
        TableData d = (TableData)data.data.GetValue();
        VRTK_SnapDropZone dropZone = ((VRTK_SnapDropZone)sender);

        SceneGameObjectReference sor = new SceneGameObjectReference(e.snappedObject.gameObject.GetComponentInChildren<SceneGameObject>());

        if (dropZone.gameObject.name == "slot00")
        {
            d.slot00 = sor;
        }
        if (dropZone.gameObject.name == "slot01")
        {
            d.slot01 = sor;
        }
        if (dropZone.gameObject.name == "slot02")
        {
            d.slot02 = sor;
        }
        if (dropZone.gameObject.name == "slot03")
        {
            d.slot03 = sor;
        }
        if (dropZone.gameObject.name == "slot04")
        {
            d.slot04 = sor;
        }

        data.data.SetValue(d);
    }

    public void SetCover()
    {
        if (!anyItem)
        {
            coverVisual.gameObject.SetActive(true);
            MessageSystem.SendMessage(data.id + "Cover On");
        }
    }
    
}
