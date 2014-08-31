using UnityEngine;
using System.Collections;

public class ObjectPoolerManager : MonoBehaviour {

    public ObjectPooler ArrowPooler;
    public ObjectPooler AudioPooler;

    public GameObject ArrowPrefab;

    public static ObjectPoolerManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (ArrowPooler == null)
        {
            GameObject go = new GameObject("ArrowPooler");
            ArrowPooler = go.AddComponent<ObjectPooler>();
            ArrowPooler.PooledObject = ArrowPrefab;
            go.transform.parent = this.gameObject.transform;
            ArrowPooler.Initialize();
        }

        if (AudioPooler == null)
        {
            GameObject go = new GameObject("AudioPooler");
            AudioPooler = go.AddComponent<ObjectPooler>();
            AudioPooler.gameObject.AddComponent<AudioSource>();
            go.transform.parent = this.gameObject.transform;
            AudioPooler.Initialize(typeof(AudioSource));
        }

        
    }

}
