using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using CultOfJakito.UltraTelephone2.Data;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    public static class FunnyBossBar
    {
        [Configgable("Patches/Silly Boss Bar", "Silly Boss Bar")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Patches/Silly Boss Bar", "Prefix Chance")]
        private static FloatSlider s_prefixChance = new FloatSlider(0.4f, 0f, 1f);

        [Configgable("Patches/Silly Boss Bar", "Suffix Chance")]
        private static FloatSlider s_suffixChance = new FloatSlider(0.15f, 0f, 1f);

        private static List<string> s_prefixes => UT2TextFiles.S_BossBarPrefixesFile.TextList;
        private static List<string> s_suffixes => UT2TextFiles.S_BossBarSuffixesFile.TextList;


        [HarmonyPatch(typeof(BossHealthBar), "Awake"), HarmonyPostfix]
        public static void OnBossBar(BossHealthBar __instance)
        {
            if (!s_enabled.Value)
                return;

            string bossName = __instance.bossName;
            //Seed is based on the current global seed, the boss name, and the current level
            int seed = UltraTelephoneTwo.Instance.Random.Seed ^ UniRandom.StringToSeed(bossName) ^ UniRandom.StringToSeed(SceneHelper.CurrentScene);

            UniRandom rng = new UniRandom(seed);

            if(rng.Chance(s_prefixChance.Value))
                bossName = rng.SelectRandomList(s_prefixes).ToUpper() + " " + bossName;

            if(rng.Chance(s_suffixChance.Value))
                bossName = bossName + " " + rng.SelectRandomList(s_suffixes).ToUpper();

            __instance.bossName = bossName;
        }
    }
}
