using UnityEngine;
using System.Collections;

public class CarrotSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}

    public GameObject Carrot;

    public void StartCarrotSpawn()
    {
        StartCoroutine(SpawnCarrot());
    }

    public void StopCarrotSpawn()
    {
        StopCoroutine(SpawnCarrot());
    }
    private IEnumerator SpawnCarrot()
    {
        while (true)
        {
            float X = Random.Range(100, Screen.width - 100);
            Vector3 randomPosition = Camera.main.ScreenToWorldPoint(new Vector3(X, 0, 0));
            GameObject carrot = Instantiate(Carrot,
                new Vector3(randomPosition.x, transform.position.y, transform.position.z),
                Quaternion.identity) as GameObject;
            carrot.GetComponent<Carrot>().FallSpeed = Random.Range(1f, 3f);
            yield return new WaitForSeconds
                (Random.Range(GameManager.Instance.MinCarrotSpawnTime, 
                GameManager.Instance.MaxCarrotSpawnTime));
        }
    }
	

}
