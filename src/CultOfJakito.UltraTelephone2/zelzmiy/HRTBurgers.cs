using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.zelzmiy;

/// <summary>
/// suggestion #314: from:bobot, "add consumable estrogen burger" <br/>
/// Replaces the Blue and red skulls with burgers that have 'T' and "E' written on them respectively
/// </summary>
[RegisterChaosEffect]
[HarmonyPatch(typeof(Skull))]
internal class HrtBurgers : ChaosEffect
{
    [Configgable("zelzmiy", "HRT Burgers")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectEnabled = false;

    private static GameObject s_estrogenBurger;
    private static GameObject s_testosteroneBurger;

    public override void BeginEffect(System.Random random)
    {
        s_estrogenBurger = UltraTelephoneTwo.Instance.ZelzmiyBundle.LoadAsset<GameObject>("estrogen burger");
        s_testosteroneBurger = UltraTelephoneTwo.Instance.ZelzmiyBundle.LoadAsset<GameObject>("testosterone burger");
        s_effectEnabled = true;
    }

    public override int GetEffectCost() => 1;

    [HarmonyPatch("Start"), HarmonyPostfix]
    public static void ReplaceSkull(Skull __instance)
    {

        if (!s_enabled.Value || !s_effectEnabled)
            return;

        //technically this is the same thing as above but i'm still doing a check in case something got fucked
        if (!s_estrogenBurger || !s_testosteroneBurger)
            return;

        // this is kind of slow but it only runs once whenever the level starts so it's prolly fine
        Renderer renderer = __instance.gameObject.GetComponent<Renderer>();
        ItemType itemType = __instance.gameObject.GetComponent<ItemIdentifier>().itemType;
        if (renderer != null)
        {
            renderer.enabled = false;
            if (itemType == ItemType.SkullRed)
            {
                Debug.Log("Instantiating estrogen burger");
                Instantiate(s_estrogenBurger, renderer.transform.position, renderer.transform.rotation, renderer.transform.parent);
            }

            if (itemType == ItemType.SkullBlue)
            {
                Debug.Log("Instantiating testosterone burger");
                Instantiate(s_testosteroneBurger, renderer.transform);
            }
        }
    }
}
