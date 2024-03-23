using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    public static class DeleteLevelsFromExistence
    {
        [Configgable("Hydra/Patches", "Disable Some Levels")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(LayerSelect), "Awake"), HarmonyPostfix]
        public static void OnAwake(LayerSelect __instance)
        {
            if (!s_enabled.Value)
                return;

            int uniqueHash = __instance.layerNumber;
            int globalSeed = UniRandom.GlobalSeed;

            UniRandom rng = new UniRandom(globalSeed^uniqueHash);

            if (rng.PercentChance(0.7f))
                return;

            LevelSelectPanel[] levelSelects = __instance.GetComponentsInChildren<LevelSelectPanel>(true);
            int deleteIndex = rng.Next(0, levelSelects.Length);
            for (int i = 0; i < levelSelects.Length; i++)
            {
                levelSelects[i].gameObject.SetActive(i!=deleteIndex);
            }
        }
    }
}
