using System.Collections;
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

        static FileSystemWatcher coconutwatcher;
        private static string s_coconutPath => Path.Combine(UT2Paths.ModFolder, fileName);
        private const string fileName = "load_bearing_coconut.png";
        public static void EnsureStability()
        {
            if (!s_enabled.Value)
                return;

            bool coconutExists = File.Exists(s_coconutPath);


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
                        if (SceneHelper.CurrentScene == "Main Menu")
                        {
                            FileTamperedWith();
                        }
                    };
                }
                else
                {
                    EnableWatcher();
                }

                return;
            }

            if (!UT2SaveData.SaveData.CoconutCreated)
            {
                File.WriteAllBytes(s_coconutPath, Properties.Resources.coconut);
                UT2SaveData.SaveData.CoconutCreated = true;
                UT2SaveData.MarkDirty();
                EnableWatcher();
            }
            else
            {
                //Write coconut back so the user isnt sad
                File.WriteAllBytes(s_coconutPath, Properties.Resources.coconut);
                Debug.LogError("COCONUT ERROR! IT WAS DELETED, reconstructing...");

                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    //Crash the game if the coconut is deleted from the main menu
                    if (SceneHelper.CurrentScene == "Main Menu")
                    {
                        FileTamperedWith();
                    }
                };

            }
        }

        private static bool ValidateCoconutIntegrity(byte[] data)
        {
            int hash = 0;
            for (int i = 0; i < data.Length; i++)
            {
                hash ^= data[i];
                hash ^= 69;
            }

            const int expectedHash = 183;
            return hash == expectedHash;
        }

        private static IEnumerator SearchForCoconutRestabilize(GameObject runner)
        {
            bool imposterCoconut = false;

            while (true)
            {
                if (File.Exists(s_coconutPath))
                {
                    if (!imposterCoconut)
                    {
                        if (ValidateCoconutIntegrity(File.ReadAllBytes(s_coconutPath)))
                        {
                            Crash.RestoreStability();
                            GameObject.Destroy(runner);
                            yield break;
                        }
                        else
                        {
                            imposterCoconut = true;
                        }
                    }
                }
                else
                {
                    imposterCoconut = false;
                }

                yield return new WaitForSecondsRealtime(0.05f);
            }

        }

        private static void EnableWatcher()
        {
            coconutwatcher = new FileSystemWatcher(UT2Paths.ModFolder, fileName);
            coconutwatcher.Changed += (_, _) => FileTamperedWith();
            coconutwatcher.Deleted += (_, _) => FileTamperedWith();
            coconutwatcher.Renamed += (_, _) => FileTamperedWith();
            coconutwatcher.EnableRaisingEvents = true;
        }

        private static void WatchForFileReturn()
        {
            CoroutineRunner cr = new GameObject("COCONUT WATCHER").AddComponent<CoroutineRunner>();
            GameObject.DontDestroyOnLoad(cr.gameObject);
            cr.StartCoroutine(SearchForCoconutRestabilize(cr.gameObject));
        }

        private static void FileTamperedWith()
        {
            if (!s_enabled.Value)
                return;

            Debug.LogError("COCONUT TAMPERED WITH!!!.. GAME IS DESTABILIZING!");
            ModalDialogue.ShowDialogue(new ModalDialogueEvent()
            {
                Title = "STABILITY COLLAPSE IMMINENT!",
                Message = "The stability of the game has been compromised. The removal or tampering of the coconut has caused the game to become unstable. Please ensure the pure form of the coconut exists or your game will become destabilized.",
                Options = new DialogueBoxOption[]
                {
                    new DialogueBoxOption()
                    {
                        Name = "I understand",
                        Color = Color.red,
                        OnClick = () =>
                        {
                            WatchForFileReturn();
                            Crash.DestabilizingCrash();
                        }
                    }
                }
            });
        }

    }
}
