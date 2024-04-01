using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2.Fun
{
    public static class LoadBearingCoconut
    {
        [Configgable("Fun", "Load Bearing Coconut")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);


        private static string s_coconutPath => Path.Combine(UT2Paths.ModFolder, "load_bearing_coconut.png");

        public static void EnsureStability()
        {
            if (!s_enabled.Value)
                return;

            bool coconutExists = File.Exists(s_coconutPath);

            //coconutwatcher = new FileSystemWatcher(UT2Paths.ModFolder, "");

            if (coconutExists)
            {
                //verify hash of coconut (very important)
                byte[] coconutBytes = File.ReadAllBytes(s_coconutPath);

                int hash = 0;

                for (int i = 0; i < coconutBytes.Length; i++)
                {
                    hash ^= coconutBytes[i];
                    hash ^= 69;
                }

                Debug.Log($"Coconut hash: {hash}");

                const int expectedHash = 183;
                if(hash != expectedHash)
                {
                    Debug.LogError("COCONUT ERROR! An Imposter....");
                    SceneManager.sceneLoaded += (scene, mode) =>
                    {
                        //Crash the game if the coconut is deleted from the main menu
                        if (scene.name == "Main Menu")
                        {
                            Application.Quit();
                        }
                    };
                }

                return;
            }

            if (!UT2SaveData.SaveData.CoconutCreated)
            {
                File.WriteAllBytes(s_coconutPath, Properties.Resources.coconut);
                UT2SaveData.SaveData.CoconutCreated = true;
                UT2SaveData.MarkDirty();
            }
            else
            {
                //Write coconut back so the user isnt sad
                File.WriteAllBytes(s_coconutPath, Properties.Resources.coconut);
                Debug.LogError("COCONUT ERROR! IT WAS DELETED, reconstructing...");

                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    //Crash the game if the coconut is deleted from the main menu
                    if (scene.name == "Main Menu")
                    {
                        Application.Quit();
                    }
                };

            }
        }

        static FileSystemWatcher coconutwatcher;

        //private static IEnumerator WatchCoconut()
        //{

        //}
    }
}
