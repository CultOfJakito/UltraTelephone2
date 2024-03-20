using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.zelzmiy;

/// <summary>
/// Suggestion #298: from: oxblood, "yuri" <br/>
/// Adds an image to the terminal
/// </summary>
[HarmonyPatch]
[RegisterChaosEffect]
internal class Yuri : ChaosEffect
{
    private static GameObject s_yuri;

    [Configgable("zelzmiy", "Yuri blocking Termianl")]
    private static ConfigToggle s_enabled = new(true);

    public override void BeginEffect(UniRandom random)
    {
        s_yuri = UltraTelephoneTwo.Instance.ZelzmiyBundle.LoadAsset<GameObject>("yuri!!!");

        if (!s_yuri)
        {
            Debug.Log("Couldn't Find Yuri!!!!");
            return;
        }
    }
    [HarmonyPatch(typeof(ShopZone), "Start"), HarmonyPostfix]
    public static void AddYuriToShop(ShopZone __instance)
    {
        if (!s_yuri || !s_enabled.Value)
            return;
        
        Canvas shopCanvas = __instance.gameObject.GetComponentInChildren<Canvas>(true);

        if (!shopCanvas)
        {
            Debug.Log("Couldn't Find Canvas!");
            return;
        }

        Debug.Log("Found Canvas, Instantiating Yuri");

        Instantiate(s_yuri, shopCanvas.transform, false);
    }

    public override int GetEffectCost() => 1;
}
