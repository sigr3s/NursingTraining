using NT;
using NT.SceneObjects;
using UnityEngine;

public class SceneObjectCollider : MonoBehaviour
{

    public bool colliding = false;
    public GameObject errorBox;
    public bool isPlacingMode = false;
    public ISceneObject assignedSo;


    private void OnTriggerEnter(Collider other) {
        if(!isPlacingMode) return;

        colliding = true;
        if(errorBox != null){
            errorBox.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(!isPlacingMode) return;
        
        colliding = false;
        if(errorBox != null){
           errorBox.SetActive(false);        
        }
    }
}