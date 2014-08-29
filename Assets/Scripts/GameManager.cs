using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    //sprites can be found here: 
    //http://www.gameartguppy.com/shop/top-tower-defense-bunny-badgers-game-art-set/

    //enemies on screen
    public List<GameObject> Enemies;
    //prefabs
    public GameObject EnemyPrefab;
    public GameObject PathPrefab;
    public GameObject TowerPrefab;
    //list of waypoints in the current level
    public Transform[] Waypoints;
    private GameObject PathPiecesParent;
    private GameObject WaypointsParent;
    //file pulled from resources
    private LevelStuffFromXML levelStuffFromXML;
    //will spawn carrots on screen
    public CarrotSpawner CarrotSpawner;

    //helpful variables for our player
    [HideInInspector]
    public int MoneyAvailable { get; private set; }
    [HideInInspector]
    public float MinCarrotSpawnTime;
    [HideInInspector]
    public float MaxCarrotSpawnTime;
    public int Lives = 10;
    private int currentRoundIndex = 0;
    [HideInInspector]
    public GameState CurrentGameState;
    public SpriteRenderer BunnyGeneratorSprite;
    [HideInInspector]
    public bool FinalRound;
    public AudioManager audioManager;
    public GUIText infoText;

    private object lockerObject = new object();

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

        FinalRound = false;
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
    private void IgnoreLayerCollisions()
    {
        int bunnyLayerID = LayerMask.NameToLayer("Bunny");
        int enemyLayerID = LayerMask.NameToLayer("Enemy");
        int arrowLayerID = LayerMask.NameToLayer("Arrow");
        int bunnyGeneratorLayerID = LayerMask.NameToLayer("BunnyGenerator");
        int backgroundLayerID = LayerMask.NameToLayer("Background");
        int pathLayerID = LayerMask.NameToLayer("Path");
        int towerLayerID = LayerMask.NameToLayer("Tower");
        int carrotLayerID = LayerMask.NameToLayer("Carrot");
        Physics2D.IgnoreLayerCollision(bunnyLayerID, enemyLayerID); //Bunny and Enemy (when dragging the bunny)
        Physics2D.IgnoreLayerCollision(arrowLayerID, bunnyGeneratorLayerID); //Arrow and BunnyGenerator
        Physics2D.IgnoreLayerCollision(arrowLayerID, backgroundLayerID); //Arrow and Background
        Physics2D.IgnoreLayerCollision(arrowLayerID, pathLayerID); //Arrow and Path
        Physics2D.IgnoreLayerCollision(arrowLayerID, bunnyLayerID); //Arrow and Bunny
        Physics2D.IgnoreLayerCollision(arrowLayerID, towerLayerID); //Arrow and Tower
        Physics2D.IgnoreLayerCollision(arrowLayerID, carrotLayerID); //Arrow and Carrot
    }



    IEnumerator NextRound()
    {
        yield return new WaitForSeconds(2f);
        Round currentRound = levelStuffFromXML.Rounds[currentRoundIndex];
        for (int i = 0; i < currentRound.NoOfEnemies; i++)
        {
            GameObject enemy = Instantiate(EnemyPrefab, Waypoints[0].position, Quaternion.identity) as GameObject;
            enemy.GetComponent<Enemy>().Speed += currentRoundIndex;
            enemy.GetComponent<Enemy>().EnemyKilled += OnEnemyKilled;
            Enemies.Add(enemy);
            yield return new WaitForSeconds(1f / (currentRoundIndex == 0 ? 1 : currentRoundIndex));
        }

    }

    void OnEnemyKilled(object sender, EventArgs e)
    {
        bool startNewRound = false;
        lock (lockerObject)
        {
            if (Enemies.Where(x => x != null).Count() == 0 && CurrentGameState == GameState.Playing)
            {
                startNewRound = true;
            }
        }
        if (startNewRound)
            CheckAndStartNewRound();
    }

    private void CheckAndStartNewRound()
    {
        if (currentRoundIndex < levelStuffFromXML.Rounds.Count - 1)
        {
            currentRoundIndex++;
            StartCoroutine(NextRound());
        }
        else
        {
            FinalRound = true;
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
                    StartCoroutine(NextRound());
                    CarrotSpawner.StartCarrotSpawn();
                }
                break;
            case GameState.Playing:
                if (Lives == 0) //we lost
                {
                    StopCoroutine(NextRound());
                    DestroyExistingEnemiesAndCarrots();
                    CarrotSpawner.StopCarrotSpawn();
                    CurrentGameState = GameState.Lost;
                }
                else if (FinalRound && Enemies.Where(x => x != null).Count() == 0)
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

    public void AlterMoneyAvailable(int money)
    {
        MoneyAvailable += money;
        if(MoneyAvailable < Constants.BunnyCost)
        {
            Color temp = BunnyGeneratorSprite.color;
            temp.a = 0.3f;
            BunnyGeneratorSprite.color = temp;
        }
        else
        {
            Color temp = BunnyGeneratorSprite.color;
            temp.a = 1.0f;
            BunnyGeneratorSprite.color = temp;
        }
    }

    void OnGUI()
    {
        Utilities.AutoResize(800, 480);
        switch (CurrentGameState)
        {
            case GameState.Start:
                infoText.text = "Tap to start!";
                break;
            case GameState.Playing:
                infoText.text = "Money: " + MoneyAvailable.ToString() + "\n"
                    + "Life: " + Lives.ToString() + "\n" +
                    string.Format("round {0} of {1}", currentRoundIndex + 1, levelStuffFromXML.Rounds.Count);
                break;
            case GameState.Won:
                infoText.text = "Won :( Tap to restart!";
                break;
            case GameState.Lost:
                infoText.text = "Lost :( Tap to restart!";
                break;
            default:
                break;
        }


    }
}
