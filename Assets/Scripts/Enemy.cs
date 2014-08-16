using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Enemy : MonoBehaviour
{

    public int Health;
    int nextWaypointIndex = 0;
    public float Speed = 1f;
    // Use this for initialization
    void Start()
    {
        Health = Constants.InitialEnemyHealth;
    }

    // Update is called once per frame
    void Update()
    {

        //calculate the distance between current position
        //and the target waypoint
        if (Vector2.Distance(transform.position, GameManager.Waypoints[nextWaypointIndex].position) < 0.01f)
        {
            //is this waypoint the last one?
            if (nextWaypointIndex == GameManager.Waypoints.Length - 1)
            {
                Destroy(this.gameObject);
                GameManager.Lives--;
            }
            else
            {
                //our enemy goes to the next waypoint
                nextWaypointIndex++;
            }
        }

        transform.LookAt(GameManager.Waypoints[nextWaypointIndex].position, -Vector3.forward);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);

        transform.position = Vector2.MoveTowards(transform.position, GameManager.Waypoints[nextWaypointIndex].position,
            Time.deltaTime * Speed);
    }




    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Arrow")
        {

            if (Health > 0)
            {

                Health -= 25;
                Debug.Log("hit " + transform.GetInstanceID() + " health " + Health);
                if (Health <= 0)
                {
                    
                    //this.gameObject.SetActive(false);
                    Destroy(this.gameObject);
                }
            }
            Destroy(col.gameObject);
        }
    }
}
