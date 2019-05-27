using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMenuOnTrigger : MonoBehaviour
{
    public GameObject menu;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player"){
            menu.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            menu.SetActive(false);
        }        
    }
}
