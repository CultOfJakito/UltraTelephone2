using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.zelzmiy;

/// <summary>
/// suggestion #314: from:bobot, "add consumable estrogen burger" <br/>
/// Replaces the Blue and red skulls with burgers that have 'T' and "E' written on them respectively
/// </summary>
[HarmonyPatch]
[RegisterChaosEffect]
internal class HrtBurgers : ChaosEffect
{
    [Configgable("Chaos/Effects", "HRT Burgers")]
    private static ConfigToggle s_enabled = new(true);

    private static GameObject s_estrogenBurger;
    private static GameObject s_testosteroneBurger;

    public override void BeginEffect(UniRandom random)
    {
        s_estrogenBurger = UT2Assets.ZelzmiyBundle.LoadAsset<GameObject>("estrogen burger");
        s_testosteroneBurger = UT2Assets.ZelzmiyBundle.LoadAsset<GameObject>("testosterone burger");
    }

    public override int GetEffectCost() => 1;

    [HarmonyPatch(typeof(Skull), "Start"), HarmonyPostfix]
    public static void ReplaceSkull(Skull __instance)
    {

        if (!s_enabled.Value)
            return;

        //technically this is the same thing as above but i'm still doing a check in case something got fucked
        if (!s_estrogenBurger || !s_testosteroneBurger)
            return;

        // this is kind of slow but it only runs once whenever the level starts so it's prolly fine
        Renderer renderer = __instance.gameObject.GetComponent<Renderer>();
        ItemType itemType = __instance.gameObject.GetComponent<ItemIdentifier>().itemType;
        if (renderer)
        {
            renderer.enabled = false;
            if (itemType == ItemType.SkullRed)
            {
                Debug.Log("Instantiating estrogen burger");
                Instantiate(s_estrogenBurger, renderer.transform);
            }

            if (itemType == ItemType.SkullBlue)
            {
                Debug.Log("Instantiating testosterone burger");
                Instantiate(s_testosteroneBurger, renderer.transform);
            }
        }
    }
}
