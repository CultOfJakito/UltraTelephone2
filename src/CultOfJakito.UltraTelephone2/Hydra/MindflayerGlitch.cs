using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    public static class MindflayerGlitch 
    {

        [Configgable("Patches", "Mindflayer Glitch")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [HarmonyPatch(typeof(Mindflayer), nameof(Mindflayer.Start)), HarmonyPostfix]
        public static void OnStart(Mindflayer __instance)
        {
            if (!s_enabled.Value)
                return;

            Transform chestTf = __instance.transform.LocateComponentInChildren<Transform>("spine.002");
            if (chestTf == null)
                return;

            GameObject bar = new GameObject("Glitch");
            AlwaysLookAtCamera alwaysLook = bar.AddComponent<AlwaysLookAtCamera>();

            alwaysLook.useXAxis = true;
            alwaysLook.useYAxis = true;
            alwaysLook.useZAxis = true;

            bar.transform.parent = chestTf;
            bar.transform.localPosition = new Vector3(0, 0.107f, 0.129f);
            bar.transform.localScale = Vector3.one;
            bar.transform.localRotation = Quaternion.identity;

            GameObject spriteBar = new GameObject("GlitchBar");
            spriteBar.transform.parent = bar.transform;
            spriteBar.transform.localPosition = new Vector3(0,0,0.167f);
            spriteBar.transform.localRotation = Quaternion.identity;
            spriteBar.transform.localScale = new Vector3(1.315981f, 0.3190258f, 0.4334f);

            SpriteRenderer rend = spriteBar.AddComponent<SpriteRenderer>();
            rend.sprite = UkPrefabs.WhiteUI.GetObject();
            rend.color = Color.black;
        }
    }
}
