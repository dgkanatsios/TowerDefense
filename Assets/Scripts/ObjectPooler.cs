using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectPooler : MonoBehaviour
{

    
    public GameObject PooledObject;
    List<GameObject> PooledObjects;
    public int PoolLength = 20;


    // Use this for initialization
    public void Initialize(params Type[] componentsToAdd)

    {
        PooledObjects = new List<GameObject>();
        for (int i = 0; i < PoolLength; i++)
        {
            GameObject go;
            if (PooledObject == null)
                go = new GameObject(this.name + " PooledObject");
            else
            {
                go = Instantiate(PooledObject) as GameObject;
            }

            foreach (var item in componentsToAdd)
            {
                go.AddComponent(item);
            }

            //go.transform.parent = this.transform;

            go.SetActive(false);
            PooledObjects.Add(go);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < PooledObjects.Count; i++)
        {
            if (!PooledObjects[i].activeInHierarchy)
            {
                return PooledObjects[i];
            }
        }
        int indexToReturn = PooledObjects.Count;
        //create more
        for (int i = 0; i < PoolLength; i++)
        {
            GameObject go = Instantiate(PooledObject) as GameObject;
            go.SetActive(false);
            PooledObjects.Add(go);
        }
        //will return the first one that we created
        return PooledObjects[indexToReturn];
    }


}
