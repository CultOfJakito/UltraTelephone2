using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    public static class FirstRoomPatch
    {

        //No suitable method found to patch so just gonna call it from main plugin class
        public static void Execute()
        {
            FirstRoomPrefab firstRoom = GameObject.FindObjectOfType<FirstRoomPrefab>();
            if (firstRoom == null)
                return;



        }
    }
}
