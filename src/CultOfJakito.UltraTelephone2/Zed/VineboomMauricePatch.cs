using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed;

[PatchThis("CultOfJakito.UltraTelephone2.Zed.VineboomMauricePatch")]
public class VineboomMauricePatch
{
    [Configgable("ZedDev/Patches", "Maurice Vine Boom")]
    private static ConfigToggle s_enabled = new(true);
    private static AudioClip s_vineBoom = UT2Assets.GetAsset<AudioClip>("Assets/Telephone 2/Misc Sounds/vineboom.mp3");

    [HarmonyPostfix, HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.OnEnable))]
    public static void PlaySound(EnemyIdentifier __instance)
    {
        if (!s_enabled.Value)
            return;

        if(!__instance.GetComponent<SpiderBody>()) return;

        if(s_vineBoom != null)
        {
            Debug.Log("Playing vineboom");
        }
        else
        {
            Debug.LogError("Vineboom clip not found");
        }
    }
}
