using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.LevelInjection.Level_4_4
{

    [RegisterLevelInjector]
    public class RoachCameo : MonoBehaviour, ILevelInjector
    {
        [Configgable("Fun", "Roach Cameo")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private const string path = "3 - Ground Floor/Secret Hall/Walls/Cube (23)";

        public void OnLevelLoaded(string sceneName)
        {
            if (sceneName != "Level 4-4" || !s_enabled.Value)
                return;

            Texture2D roach = HydraAssets.RoachCarving;
            if(roach == null)
            {
                Debug.LogError("Roach not exist.");
                return;
            }

            MeshRenderer meshRenderer = Locator.LocateComponent<MeshRenderer>(path);
            if (meshRenderer == null)
            {
                Debug.LogError("Dreamed not found.");
                return;
            }

            //This is dreamed.
            Material dreamedMateiral = meshRenderer.materials[4];
            dreamedMateiral.mainTexture = roach;
            //Is now roach.
            Debug.Log("Roach made");
        }
    }
}
