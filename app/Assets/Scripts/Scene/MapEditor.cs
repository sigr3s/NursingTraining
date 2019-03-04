using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    // Start is called before the first frame update
    public int xSize = 20;
    public int ySize = 20;
    Camera c;

    public MapTile mp;
    public GameObject placeMentObject;

    void Start()
    {
        c = Camera.main;

        for(int i = -xSize/2; i < xSize/2; i++){
            for(int j = -ySize/2; j < ySize/2; j++){
                var cp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cp.transform.parent = this.transform;
                cp.transform.position = new Vector3(i,0,j);
                cp.AddComponent<MapTile>();
            }
        }
    }

    private void Update() {
        RaycastHit hit;
        Ray ray = c.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            if(objectHit != null){
                MapTile mt = objectHit.GetComponent<MapTile>();
                if(mt != null){
                    if(mp != null){
                        mp.GetComponent<Renderer>().material.color = Color.white;
                    }

                    mt.GetComponent<Renderer>().material.color = Color.blue;
                    placeMentObject.transform.position = mt.transform.position;
                    placeMentObject.transform.position += new Vector3(0,1,0);
                    mp = mt;

                    if(Input.GetMouseButtonDown(0)){
                        GameObject g = Instantiate(placeMentObject);
                    }
                }
            }
            // Do something with the object that was hit by the raycast.
        }
    }

}
