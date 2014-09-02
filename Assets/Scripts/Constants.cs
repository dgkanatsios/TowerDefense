using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Constant helper variables
    /// </summary>
    public static class Constants
    {
        public static readonly Color RedColor = new Color(1f, 0f, 0f, 0f);
        public static readonly Color BlackColor = new Color(0f, 0f, 0f, 0f);
        public static readonly int BunnyCost = 50;
        public static readonly int CarrotAward = 10;
        public static readonly int InitialEnemyHealth = 50;
        public static readonly int ArrowDamage = 20;
        public static readonly float MinDistanceForBunnyToShoot = 3f;
        
    }
}
