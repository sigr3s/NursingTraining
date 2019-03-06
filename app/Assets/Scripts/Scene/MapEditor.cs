using System.Collections;
using System.Collections.Generic;
using NT;
using NT.SceneObjects;
using UnityEngine;


public class MapEditor : MonoBehaviour
{
    public Camera c;

    public float gridSize = 0.1f;
    public int selectedObject  = 0;
    public List<DummySceneObject> objectSet;
    public LayerMask floorOnly;

    
    private GameObject items;
    void Start()
    {
        items = new GameObject();
        items.transform.parent = this.transform;
        items.transform.localPosition = Vector3.zero; 
    }
    
    public GameObject previewGO = null;
    public SceneObjectCollider previewGOSC = null;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.N)){
            if(previewGO != null){
                Destroy(previewGO);
            }

            selectedObject++;

            if(selectedObject > objectSet.Count-1) selectedObject = 0;

        }

        if(previewGO == null){
            previewGO = Instantiate(objectSet[selectedObject].model);
            
            previewGOSC = previewGO.GetComponent<SceneObjectCollider>();

            if(previewGOSC == null){
                previewGOSC = previewGO.AddComponent<SceneObjectCollider>();
            }
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            previewGO.transform.Rotate(new Vector3(0,1,0), 90);
        }

        if(Input.GetKeyDown(KeyCode.E)){
            previewGO.transform.Rotate(new Vector3(0,1,0), -90);
        }

        RaycastHit hit;
        Ray ray = c.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit,50,floorOnly)) {
            Transform objectHit = hit.transform;
            if(objectHit != null){
                Vector2 hitPointOnPlane = new Vector2(hit.point.x, hit.point.z);
                float x = hitPointOnPlane.x - hitPointOnPlane.x%gridSize;  
                float z = hitPointOnPlane.y - hitPointOnPlane.y%gridSize; 

                previewGO.transform.position = new Vector3(x, hit.point.y, z);

                if(Input.GetMouseButtonDown(0) && !previewGOSC.colliding){
                    GameObject item = Instantiate(previewGO,items.transform);
                } 
            }
        }
    }

}
