using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//disable it after 5 seconds, whatever happens
		Invoke("Disable", 5f);
	}

	public void Disable()
	{
		//if we are called from another gameobject,
		//cancel the timed invoke
		CancelInvoke();
		//since we're pooling it, make it inactive instead of destroying it
		this.gameObject.SetActive(false);
	}
	
	
}
