using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Zed;

[PatchThis("CultOfJakito.UltraTelephone2.Zed.VineboomMauricePatch")]
public class VineboomMauricePatch
{
    [HarmonyPatch(typeof(EnemyIdentifier), "OnEnable")]
    [HarmonyPostfix]
    public static void PlaySound(EnemyIdentifier __instance)
    {
        if(!__instance.GetComponent<SpiderBody>()) return;
        AudioClip? vineboom = ZedResources.GetCached<AudioClip>("vineboom");
        if(vineboom != null)
        {
            Debug.Log("Playing vineboom");
            vineboom.PlaySound(position: __instance.transform.position);
        }
        else
        {
            Debug.LogError("Vineboom clip not found");
        }
    }
}