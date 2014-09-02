using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Carrot : MonoBehaviour
{

    //carrot sprite found in http://opengameart.org/content/easter-carrot-pick-up-item

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
            //check if the user tap/click touches the carrot
            Vector2 location = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (this.GetComponent<BoxCollider2D>() == Physics2D.OverlapPoint(location,
                1 << LayerMask.NameToLayer("Carrot")))
            {
                //increase player money
                GameManager.Instance.AlterMoneyAvailable(Constants.CarrotAward);
                //destroy carrot
                Destroy(this.gameObject);
            }
        }
    }

    public float FallSpeed = 1;
}
