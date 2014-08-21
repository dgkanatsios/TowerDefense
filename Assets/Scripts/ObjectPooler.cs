using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler Instance;
    public GameObject PooledObject;
    List<GameObject> PooledObjects;
    public int PoolLength = 20;
    public bool AddAudioSource = true;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        PooledObjects = new List<GameObject>();
        for (int i = 0; i < PoolLength; i++)
        {
            GameObject go;
            if (PooledObject == null)
                go = new GameObject("PooledObject");
            else
                go = Instantiate(PooledObject) as GameObject;
            if (AddAudioSource)
                go.AddComponent<AudioSource>();
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
