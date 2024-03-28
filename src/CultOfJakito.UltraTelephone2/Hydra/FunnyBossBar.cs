using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Configgy;
using CultOfJakito.UltraTelephone2.Util;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    public static class FunnyBossBar
    {
        [Configgable("Patches/Silly Boss Bar", "Silly Boss Bar")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Patches/Silly Boss Bar", "Prefix Chance")]
        private static FloatSlider s_prefixChance = new FloatSlider(0.25f, 0f, 1f);

        [Configgable("Patches/Silly Boss Bar", "Suffix Chance")]
        private static FloatSlider s_suffixChance = new FloatSlider(0.25f, 0f, 1f);

        [Configgable("Patches/Silly Boss Bar", "Reload Text Files")]
        private static ConfigButton s_reloadFiles = new ConfigButton(ReloadFiles, "Reload Text Files");

        private static string funnyBossBarPrefixesFilePath => Path.Combine(UT2Paths.DataFolder, "funnyBossBar.prefixes.txt");
        private static string funnyBossBarSuffixesFilePath => Path.Combine(UT2Paths.DataFolder, "funnyBossBar.suffixes.txt");

        private static List<string> s_prefixes = new List<string>();
        private static List<string> s_suffixes = new List<string>();

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

        public static void ReloadFiles()
        {
            if (!File.Exists(funnyBossBarPrefixesFilePath))
            {
                string builtInText = Properties.Resources.funnyBossBar_prefixes;
                File.WriteAllText(funnyBossBarPrefixesFilePath, builtInText);
                s_prefixes = new List<string>(builtInText.Split('\n'));
            }
            else
            {
                string text = File.ReadAllText(funnyBossBarPrefixesFilePath);
                s_prefixes = new List<string>(text.Split('\n'));
            }

            if (!File.Exists(funnyBossBarSuffixesFilePath))
            {
                string builtInText = Properties.Resources.funnyBossBar_suffixes;
                File.WriteAllText(funnyBossBarSuffixesFilePath, builtInText);
                s_suffixes = new List<string>(builtInText.Split('\n'));
            }
            else
            {
                string text = File.ReadAllText(funnyBossBarSuffixesFilePath);
                s_suffixes = new List<string>(text.Split('\n'));
            }
        }

    }
}
