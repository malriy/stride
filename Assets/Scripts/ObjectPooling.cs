using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public GameObject pooledObject; // The object to be pooled
    public int pooledAmount;   // The number of instances to initially create

    private List<GameObject> pooledObjects = new List<GameObject>();

    void Start()
    {
        // Instantiate and pool the objects
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    // Retrieve a pooled object
    public GameObject GetPooledObject()
    {
        // Iterate through the list of pooled objects
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // If the object is not active, return it
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        // If all objects are in use, create a new one (expand the pool if desired)
        GameObject newObj = Instantiate(pooledObject);
        newObj.SetActive(false);
        pooledObjects.Add(newObj);
        return newObj;
    }

    // Other methods or functions related to object pooling can be added here
}