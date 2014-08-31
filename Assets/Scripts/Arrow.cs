using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("Disable", 5f);
	}

    public void Disable()
    {
        this.gameObject.SetActive(false);
    }
	
	
}
