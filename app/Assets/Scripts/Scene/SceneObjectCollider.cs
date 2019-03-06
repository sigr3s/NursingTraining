using UnityEngine;

public class SceneObjectCollider : MonoBehaviour
{

    public bool colliding = false;
    public GameObject errorBox;


    private void OnTriggerEnter(Collider other) {
       colliding = true;
       if(errorBox != null){
           errorBox.SetActive(true);
       }
    }

    private void OnTriggerExit(Collider other) {
        colliding = false;
        if(errorBox != null){
           errorBox.SetActive(false);        
        }
    }
}