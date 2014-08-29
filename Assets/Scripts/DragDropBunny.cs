using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts;

public class DragDropBunny : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
    }
   
    private Camera mainCamera;
    public GameObject BunnyPrefab;
    public GameObject BunnyGenerator;
    bool isDragging = false;
    private GameObject newBunny;

    private GameObject tempBackgroundBehindPath;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging &&
            GameManager.Instance.MoneyAvailable >= Constants.BunnyCost)
        {
            ResetTempBackgroundColor();
            Vector2 location = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            //if user has tapped onto the bunny generator
            if (BunnyGenerator.GetComponent<CircleCollider2D>() ==
                Physics2D.OverlapPoint(location, 1 << LayerMask.NameToLayer("BunnyGenerator")))
            {
                isDragging = true;
                newBunny = Instantiate(BunnyPrefab, BunnyGenerator.transform.position, Quaternion.identity)
                    as GameObject;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);
            if (hits.Length > 0 && hits[0].collider != null)
            {
                newBunny.transform.position = hits[0].collider.gameObject.transform.position;

                //if we're hitting a path or tower
                //or there is an existing bunny there
                //we use > 1 since we're hovering over the newBunny gameobject :)
                if (hits.Where(x => x.collider.gameObject.tag == "Path"
                    || x.collider.gameObject.tag == "Tower").Count() > 0
                    || hits.Where(x=>x.collider.gameObject.tag == "Bunny").Count() > 1)
                {
                    //we cannot place a bunny there
                    GameObject backgroundBehindPath = hits.Where(x => x.collider.gameObject.tag == "Background").First().collider.gameObject;
                    backgroundBehindPath.GetComponent<SpriteRenderer>().color = Constants.RedColor;
                    
                    if (tempBackgroundBehindPath != backgroundBehindPath)
                        ResetTempBackgroundColor();
                    
                    tempBackgroundBehindPath = backgroundBehindPath;
                }
                else //just reset the color on previously set paths
                {
                    ResetTempBackgroundColor();
                }

            }

        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {

            ResetTempBackgroundColor();
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction,
                Mathf.Infinity, ~(1 << LayerMask.NameToLayer("BunnyGenerator")));
            //in order to place it, we must have a background and no other bunnies
            if (hits.Where(x=>x.collider.gameObject.tag == "Background").Count() > 0
                && hits.Where(x => x.collider.gameObject.tag == "Path").Count() == 0
                && hits.Where(x=>x.collider.gameObject.tag == "Bunny").Count() == 1)
            {
                GameManager.Instance.AlterMoneyAvailable(-Constants.BunnyCost);
                newBunny.transform.position = hits.Where(x => x.collider.gameObject.tag == "Background")
                    .First().collider.gameObject.transform.position;
                newBunny.GetComponent<Bunny>().Activate();
            }
            else
            {
                Destroy(newBunny);
            }
            isDragging = false;

        }
    }


    private void ResetTempBackgroundColor()
    {
        if (tempBackgroundBehindPath != null)
            tempBackgroundBehindPath.GetComponent<SpriteRenderer>().color = Constants.BlackColor;
    }

}
