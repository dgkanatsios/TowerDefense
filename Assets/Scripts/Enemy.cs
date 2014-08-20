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
                DestroyAndRemoveFromMemory();
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
        {//if we're hit by an arrow
            if (Health > 0)
            {
                Health -= Constants.ArrowDamage; 
                if (Health <= 0)
                {
                    DestroyAndRemoveFromMemory();
                }
            }
            Destroy(col.gameObject); //destroy the arrow
        }
    }

    void DestroyAndRemoveFromMemory()
    {
        Destroy(this.gameObject);
        GameManager.Enemies.Remove(this.gameObject);
    }
}
