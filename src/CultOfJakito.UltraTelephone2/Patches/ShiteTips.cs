using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.Placeholders;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class ShiteTips
    {
        [Configgable("Patches", "Shite Terminal Tips")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);
        private static List<string> tips => UT2TextFiles.ShiteTipsFile.TextList;

        [HarmonyPatch(typeof(ShopZone), nameof(ShopZone.Start)), HarmonyPostfix]
        public static void OnShopZoneStart(ShopZone __instance)
        {
            if (!s_enabled.Value)
                return;

            TextMeshProUGUI tmp = __instance.transform.LocateComponentInChildren<TextMeshProUGUI>("TipText");

            if (tmp == null)
                return;

            int globalSeed = UltraTelephoneTwo.Instance.Random.Seed;
            Vector3 position = __instance.transform.position;
            int sceneSeed = UniRandom.StringToSeed(SceneHelper.CurrentScene);

            int seed = globalSeed ^ UniRandom.StringToSeed(position.ToString()) ^ sceneSeed;

            UniRandom rng = new UniRandom(seed);
            string tip = rng.SelectRandom(tips);
            tip = PlaceholderHelper.ReplacePlaceholders(tip);
            tmp.text = tip;
        }
    }
}
