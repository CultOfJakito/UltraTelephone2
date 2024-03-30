using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class SillyNames : ChaosEffect
    {
        [Configgable("Patches/Silly Names", "Silly Names")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Patches/Silly Names", "Prefix Chance")]
        private static FloatSlider s_prefixChance = new FloatSlider(0.4f, 0f, 1f);

        [Configgable("Patches/Silly Names", "Suffix Chance")]
        private static FloatSlider s_suffixChance = new FloatSlider(0.15f, 0f, 1f);

        private static List<string> s_prefixes => UT2TextFiles.S_BossBarPrefixesFile.TextList;
        private static List<string> s_suffixes => UT2TextFiles.S_BossBarSuffixesFile.TextList;

        private static bool s_effectActive;

        public static string SillifyName(string name)
        {
            if (!s_enabled.Value || !s_effectActive)
                return name;

            //Seed is based on the current global seed, the boss name, and the current level
            int seed = new SeedBuilder()
                .WithGlobalSeed()
                .WithSeed(name)
                .WithSceneName()
                .GetSeed();

            UniRandom rng = new UniRandom(seed);

            if (rng.Chance(s_prefixChance.Value))
                name = rng.SelectRandom(s_prefixes) + " " + name;

            if (rng.Chance(s_suffixChance.Value))
                name = name + " " + rng.SelectRandom(s_suffixes);

            return name;
        }

        [HarmonyPatch(typeof(BossHealthBar), "Awake"), HarmonyPostfix]
        public static void OnBossBar(BossHealthBar __instance)
        {
            if (!s_enabled.Value)
                return;

            string bossName = __instance.bossName;
            __instance.bossName = SillifyName(bossName).ToUpper();
        }

        public override void BeginEffect(UniRandom random)
        {
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
        public override int GetEffectCost() => 1;

        protected override void OnDestroy() => s_effectActive = false;
    }
}
