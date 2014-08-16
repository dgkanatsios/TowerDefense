using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class LevelStuffFromXML
    {
        public float MinCarrotSpawnTime;
        public float MaxCarrotSpawnTime;
        public int InitialMoney;
        public List<Round> Rounds;
        public List<Vector2> Paths;
        public List<Vector2> Waypoints;
        public Vector2 Tower;
        public LevelStuffFromXML()
        {
            Paths = new List<Vector2>();
            Waypoints = new List<Vector2>();
            Rounds = new List<Round>();
        }

    }

    public class Round
    {
        public int NoOfEnemies { get; set; }
    }


    
}
