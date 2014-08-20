using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using System.Collections;

public class GameManager : MonoBehaviour
{

    //sprites can be found here: 
    //http://www.gameartguppy.com/shop/top-tower-defense-bunny-badgers-game-art-set/

    //enemies on screen
    public static List<GameObject> Enemies;
    //prefabs
    public GameObject EnemyPrefab;
    public GameObject PathPrefab;
    public GameObject TowerPrefab;
    //list of waypoints in the current level
    public static Transform[] Waypoints;
    private GameObject PathPiecesParent;
    private GameObject WaypointsParent;
    //file pulled from resources
    private LevelStuffFromXML levelStuffFromXML;
    //will spawn carrots on screen
    public CarrotSpawner CarrotSpawner;

    //helpful variables for our player
    public static int MoneyAvailable;
    public static float MinCarrotSpawnTime, MaxCarrotSpawnTime;
    public static int Lives = 10;
    private int currentRoundIndex = 0;
    public static GameState CurrentGameState;

    public static bool FinalEnemyRound;

    public GUIText guiText;

    // Use this for initialization
    void Start()
    {

        IgnoreLayerCollisions();

        Enemies = new List<GameObject>();
        PathPiecesParent = GameObject.Find("PathPieces");
        WaypointsParent = GameObject.Find("Waypoints");
        levelStuffFromXML = Utilities.ReadXMLFile();

        CreateLevelFromXML();

        CurrentGameState = GameState.Start;

        FinalEnemyRound = false;
    }

    /// <summary>
    /// Will create necessary stuff from the object that has the XML stuff
    /// </summary>
    private void CreateLevelFromXML()
    {
        foreach (var position in levelStuffFromXML.Paths)
        {
            GameObject go = Instantiate(PathPrefab, position, Quaternion.identity) as GameObject;
            go.GetComponent<SpriteRenderer>().sortingLayerName = "Path";
            go.transform.parent = PathPiecesParent.transform;
        }

        for (int i = 0; i < levelStuffFromXML.Waypoints.Count; i++)
        {
            GameObject go = new GameObject();
            go.transform.position = levelStuffFromXML.Waypoints[i];
            go.transform.parent = WaypointsParent.transform;
            go.tag = "Waypoint";
            go.name = "Waypoints" + i.ToString();
        }

        GameObject tower = Instantiate(TowerPrefab, levelStuffFromXML.Tower, Quaternion.identity) as GameObject;
        tower.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";

        Waypoints = GameObject.FindGameObjectsWithTag("Waypoint")
            .OrderBy(x => x.name).Select(x => x.transform).ToArray();

        MoneyAvailable = levelStuffFromXML.InitialMoney;
        MinCarrotSpawnTime = levelStuffFromXML.MinCarrotSpawnTime;
        MaxCarrotSpawnTime = levelStuffFromXML.MaxCarrotSpawnTime;
    }

    /// <summary>
    /// Will make the arrow collide only with enemies!
    /// </summary>
    private static void IgnoreLayerCollisions()
    {
        Physics2D.IgnoreLayerCollision(12, 15); //Bunny and Enemy (when dragging the bunny)
        Physics2D.IgnoreLayerCollision(9, 8); //Arrow and BunnyGenerator
        Physics2D.IgnoreLayerCollision(9, 10); //Arrow and Background
        Physics2D.IgnoreLayerCollision(9, 11); //Arrow and Path
        Physics2D.IgnoreLayerCollision(9, 12); //Arrow and Bunny
        Physics2D.IgnoreLayerCollision(9, 13); //Arrow and Tower
        Physics2D.IgnoreLayerCollision(9, 14); //Arrow and Carrot
    }



    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            //before spawning, check if there are active enemies on screen
            if (Enemies.Where(x => x != null).Count() > 0 && !FinalEnemyRound)
            {
                //enemies exist, so wait for 2 seconds and check again
                yield return new WaitForSeconds(2f);
                continue;
            }

            Round currentRound = levelStuffFromXML.Rounds[currentRoundIndex];
            for (int i = 0; i < currentRound.NoOfEnemies; i++)
            {
                GameObject enemy = Instantiate(EnemyPrefab, Waypoints[0].position, Quaternion.identity) as GameObject;
                Enemies.Add(enemy);
                yield return new WaitForSeconds(1f);
            }
            if (currentRoundIndex < levelStuffFromXML.Rounds.Count - 1)
            {
                currentRoundIndex++;
            }
            else
            {
                FinalEnemyRound = true;
                yield break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.Start:
                if (Input.GetMouseButtonUp(0))
                {
                    CurrentGameState = GameState.Playing;
                    StartCoroutine(SpawnEnemy());
                    CarrotSpawner.StartCarrotSpawn();
                }
                break;
            case GameState.Playing:
                if (Lives == 0) //we lost
                {
                    StopCoroutine(SpawnEnemy());
                    DestroyExistingEnemiesAndCarrots();
                    CarrotSpawner.StopCarrotSpawn();
                    CurrentGameState = GameState.Lost;
                }
                else if (FinalEnemyRound && Enemies.Where(x => x != null).Count() == 0)
                {
                    DestroyExistingEnemiesAndCarrots();
                    CarrotSpawner.StopCarrotSpawn();
                    CurrentGameState = GameState.Won;
                }
                break;
            case GameState.Won:
                if (Input.GetMouseButtonUp(0))
                {
                    Application.LoadLevel(Application.loadedLevel);
                }
                break;
            case GameState.Lost:
                if (Input.GetMouseButtonUp(0))
                {
                    Application.LoadLevel(Application.loadedLevel);
                }
                break;
            default:
                break;
        }
    }

    private void DestroyExistingEnemiesAndCarrots()
    {
        foreach (var item in Enemies)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        var carrots = GameObject.FindGameObjectsWithTag("Carrot");
        foreach (var item in carrots)
        {
            Destroy(item);
        }
    }

    void OnGUI()
    {
        Utilities.AutoResize(800, 480);
        switch (CurrentGameState)
        {
            case GameState.Start:
                guiText.text = "Tap to start!";
                break;
            case GameState.Playing:
                guiText.text = "Money: " + MoneyAvailable.ToString() + "\n"
                    + "Life: " + Lives.ToString();
                break;
            case GameState.Won:
                guiText.text = "Won :( Tap to restart!";
                break;
            case GameState.Lost:
                guiText.text = "Lost :( Tap to restart!";
                break;
            default:
                break;
        }


    }
}
