using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using NT;


public class GlovesSceneGameObjects : SceneGameObject
{
    public Color glovesColor;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<VRTK.VRTK_AvatarHandController>() != null)
        {
            VRTK_AvatarHandController[] hands = FindObjectsOfType<VRTK_AvatarHandController>();

            foreach(var h in hands)
            {
                h.GetComponentInChildren<SkinnedMeshRenderer>().material.color = glovesColor;      
            }
            MessageSystem.SendMessage(data.id + "Gloves On");
            gameObject.SetActive(false);
        }
    }
}
