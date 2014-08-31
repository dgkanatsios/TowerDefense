using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectPooler : MonoBehaviour
{

    public Transform Parent;
    public GameObject PooledObject;
    private List<GameObject> PooledObjects;
    public int PoolLength = 10;

    private Type[] componentsToAdd;

    public void Initialize()
    {
        PooledObjects = new List<GameObject>();
        for (int i = 0; i < PoolLength; i++)
        {
            CreateObjectsInPool();
        }
    }

    public void Initialize(params Type[] componentsToAdd)
    {
        this.componentsToAdd = componentsToAdd;
        Initialize();
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
        CreateObjectsInPool(); Debug.Log("created more");
        //will return the first one that we created
        return PooledObjects[indexToReturn];
    }

    private void CreateObjectsInPool()
    {
        GameObject go;
        if (PooledObject == null)
            go = new GameObject(this.name + " PooledObject");
        else
        {
            go = Instantiate(PooledObject) as GameObject;
        }

        go.SetActive(false);
        PooledObjects.Add(go);

        if (componentsToAdd != null)
            foreach (var item in componentsToAdd)
            {
                go.AddComponent(item);
            }


        if (Parent != null)
            go.transform.parent = this.Parent;


    }
}
