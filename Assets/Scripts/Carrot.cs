using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Carrot : MonoBehaviour
{

    //sprite found in http://opengameart.org/content/easter-carrot-pick-up-item

    Camera mainCamera;

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y - Time.deltaTime * FallSpeed, 
            transform.position.z);
        transform.Rotate(0, 0, Time.deltaTime * 10);

        if (Input.GetMouseButtonDown(0))
        {

            Vector2 location = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            //14 is tha layer ID for the carrot layer
            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(location, 1<<14))
            {
                GameManager.Instance.AlterMoneyAvailable(Constants.CarrotAward);
                Destroy(this.gameObject);
            }
        }
    }

    public float FallSpeed = 1;
}
