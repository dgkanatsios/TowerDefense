using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Xml.Linq;
using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;

public class LevelExport : EditorWindow
{
    [MenuItem("Custom Editor/Export Level")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelExport));
    }

    Vector2 scrollPosition = Vector2.zero;
    int noOfEnemies;
    int initialMoney;
    int MinCarrotSpawnTime, MaxCarrotSpawnTime;
    string filename = "LevelX.xml";
    int waypointsCount;
    int pathsCount;
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.LabelField("Total Rounds created:" + rounds.Count);
        for (int i = 0; i < rounds.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Round " + (i + 1));
            EditorGUILayout.LabelField("Number of Enemies " + rounds[i].NoOfEnemies);
            if (GUILayout.Button("Delete"))
            {
                rounds.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.LabelField("Add a new round", EditorStyles.boldLabel);
        noOfEnemies = EditorGUILayout.IntSlider("Number of enemies", noOfEnemies, 1, 20);

        if (GUILayout.Button("Add new round"))
        {
            rounds.Add(new Round() { NoOfEnemies = noOfEnemies });
        }
        initialMoney = EditorGUILayout.IntSlider("Initial Money", initialMoney, 200, 400);
        MinCarrotSpawnTime = EditorGUILayout.IntSlider("MinCarrotSpawnTime", MinCarrotSpawnTime, 1, 10);
        MaxCarrotSpawnTime = EditorGUILayout.IntSlider("MaxCarrotSpawnTime", MaxCarrotSpawnTime, 1, 10);
        filename = EditorGUILayout.TextField("Filename:", filename);
        EditorGUILayout.LabelField("Export Level", EditorStyles.boldLabel);
        if (GUILayout.Button("Export"))
        {
            Export();
        }
    }

    XDocument doc;
    List<Round> rounds = new List<Round>();

    // The export method
    void Export()
    {
        // Create a new output file stream
        doc = new XDocument();
        doc.Add(new XElement("Elements"));
        XElement elements = doc.Element("Elements");


        XElement pathPiecesXML = new XElement("PathPieces");
        var paths = GameObject.FindGameObjectsWithTag("Path");
       
        foreach (var item in paths)
        {
            XElement path = new XElement("Path");
            XAttribute attrX = new XAttribute("X", item.transform.position.x);
            XAttribute attrY = new XAttribute("Y", item.transform.position.y);
            path.Add(attrX, attrY);
            pathPiecesXML.Add(path);
        }
        pathsCount = paths.Length;
        elements.Add(pathPiecesXML);

        XElement waypointsXML = new XElement("Waypoints");
        var waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        if (!WaypointsAreValid(waypoints))
        {
            return;
        }
        //order by user selected order
        waypoints = waypoints.OrderBy(x => x.GetComponent<OrderedWaypointForEditor>().Order).ToArray();
        foreach (var item in waypoints)
        {
            XElement waypoint = new XElement("Waypoint");
            XAttribute attrX = new XAttribute("X", item.transform.position.x);
            XAttribute attrY = new XAttribute("Y", item.transform.position.y);
            waypoint.Add(attrX, attrY);
            waypointsXML.Add(waypoint);
        }
        waypointsCount = waypoints.Length;
        elements.Add(waypointsXML);

        XElement roundsXML = new XElement("Rounds");
        foreach (var item in rounds)
        {
            XElement round = new XElement("Round");
            XAttribute NoOfEnemies = new XAttribute("NoOfEnemies", item.NoOfEnemies);
            round.Add(NoOfEnemies);
            roundsXML.Add(round);
        }
        elements.Add(roundsXML);

        XElement towerXML = new XElement("Tower");
        var tower = GameObject.FindGameObjectWithTag("Tower");
        if(tower == null)
        {
            ShowErrorForNull("Tower");
            return;
        }
        XAttribute towerX = new XAttribute("X", tower.transform.position.x);
        XAttribute towerY = new XAttribute("Y", tower.transform.position.y);
        towerXML.Add(towerX, towerY);
        elements.Add(towerXML);

        XElement otherStuffXML = new XElement("OtherStuff");
        otherStuffXML.Add(new XAttribute("InitialMoney", initialMoney));
        otherStuffXML.Add(new XAttribute("MinCarrotSpawnTime", MinCarrotSpawnTime));
        otherStuffXML.Add(new XAttribute("MaxCarrotSpawnTime", MaxCarrotSpawnTime));
        elements.Add(otherStuffXML);


        if (!InputIsValid())
            return;



        if (EditorUtility.DisplayDialog("Save confirmation",
            "Are you sure you want to save level " + filename +"?", "OK", "Cancel"))
        {
            doc.Save("Assets/" + filename);
            EditorUtility.DisplayDialog("Saved", filename + " saved!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("NOT Saved", filename + " not saved!", "OK");
        }
    }

    private bool WaypointsAreValid(GameObject[] waypoints)
    {
        //first check whether whey all have a OrderedWaypoint component
        if (!waypoints.All(x => x.GetComponent<OrderedWaypointForEditor>() != null))
        {
            EditorUtility.DisplayDialog("Error", "All waypoints must have an ordered waypoint component", "OK");
            return false;
        }
        //check if all Order fields on the orderwaypoint components are different

        if (waypoints.Count() != waypoints.Select(x=>x.GetComponent<OrderedWaypointForEditor>().Order).Distinct().Count())
        {
            EditorUtility.DisplayDialog("Error", "All waypoints must have a different order", "OK");
            return false;
        }
        return true;
    }

    private void ShowErrorForNull(string gameObjectName)
    {
        EditorUtility.DisplayDialog("Error", "Cannot find gameobject " + gameObjectName, "OK");
    }

    private bool InputIsValid()
    {
        if (MinCarrotSpawnTime > MaxCarrotSpawnTime)
        {
            EditorUtility.DisplayDialog("Error", "MinCarrotSpawnTime must be less or equal "
            + " to MaxCarrotSpawnTime", "OK");
            return false;
        }

        if (rounds.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "You cannot have 0 rounds", "OK");
            return false;
        }

        if (waypointsCount == 0)
        {
            EditorUtility.DisplayDialog("Error", "You cannot have 0 waypoints", "OK");
            return false;
        }

        if (pathsCount == 0)
        {
            EditorUtility.DisplayDialog("Error", "You cannot have 0 paths", "OK");
            return false;
        }

        return true;
    }

}
