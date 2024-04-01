using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class EnterCasinoPatch
    {
        public const string CASINO_SCENE_NAME = "Assets/Telephone 2/Scenes/CASINO.unity";

        [Configgable("Fun", "Casino")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.LevelChange)), HarmonyPrefix]
        private static bool OnFinalPit(FinalRank __instance)
        {
            UniRandom rand = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSeed(StatsManager.Instance.kills));

            float chance = 0.33f;

            if (!UT2SaveData.SaveData.BeenToCasino)
                chance = 0.8f;

            if (!rand.Chance(chance))
                return true;

            UT2SaveData.SaveData.BeenToCasino = true;
            UT2SaveData.MarkDirty();

            AddressableManager.LoadSceneUnsanitzed(CASINO_SCENE_NAME);

            return false;
        }
    }
}
