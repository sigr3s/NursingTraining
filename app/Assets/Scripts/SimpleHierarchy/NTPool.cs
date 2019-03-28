using System.Collections.Generic;
using UnityEngine;

public class NTPool : MonoBehaviour {
    bool initialized = false;
    public GameObject prefab;

    public List<GameObject> pooledObjects = new List<GameObject>();


    public void Initialize( GameObject prefab, int initialCapacity = 10){
        this.prefab = prefab;

        for (int i = 00; i < initialCapacity; i++)
        {
            GameObject pooledObject = Instantiate(prefab, this.transform);
            pooledObject.SetActive(false);

            pooledObjects.Add(pooledObject);
        }

        initialized = true;
    }

    public void PoolItem(GameObject activeObject){
        if(!initialized) return;

        activeObject.SetActive(false);
        activeObject.transform.SetParent(transform);
        pooledObjects.Add(activeObject);
    }


    public GameObject GetItem(){
        GameObject pooledObject = null;

        if(!initialized) return null;


        if(pooledObjects.Count > 0){
            pooledObject = pooledObjects[pooledObjects.Count - 1];
            pooledObject.SetActive(true);

            pooledObjects.RemoveAt(pooledObjects.Count -1);

        }
        else
        {
            pooledObject = Instantiate(prefab, this.transform);
        }

        return pooledObject;
    }
}