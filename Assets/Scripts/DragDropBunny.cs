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
        if (Input.GetMouseButtonDown(0) && !isDragging && GameManager.MoneyAvailable >= Constants.BunnyCost)
        {
            ResetTempBackgroundColor();
            Vector2 location = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            //if user has tapped onto the bunny generator
            //8 is the layerID of the BunnyGenerator
            if (BunnyGenerator.GetComponent<CircleCollider2D>() == Physics2D.OverlapPoint(location, 1 << 8))
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

                //if we're hitting a path
                if (hits.Where(x => x.collider.gameObject.tag == "Path" || x.collider.gameObject.tag == "Tower").Count() > 0)
                {
                    //we cannot place a bunny on the path
                    GameObject backgroundBehindPath = hits.Where(x => x.collider.gameObject.tag == "Background").Single().collider.gameObject;
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
            //8 is BunnyGenerator layer ID
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, ~(1 << 8));
            if (hit.collider.gameObject.tag == "Background")
            {
                GameManager.MoneyAvailable -= Constants.BunnyCost;
                newBunny.transform.position = hit.collider.gameObject.transform.position;
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
