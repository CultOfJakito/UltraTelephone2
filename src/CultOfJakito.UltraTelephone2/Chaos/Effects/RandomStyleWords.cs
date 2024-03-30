using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
[HarmonyPatch]
public class RandomStyleWords : ChaosEffect
{
    [Configgable("Chaos/Effects", "RandomStyleWords")]
    public static ConfigToggle Enabled = new(true);

    private static bool s_currentlyActive;
    private static List<string> wordlist => UT2TextFiles.S_WordList10k.TextList;

    public override void BeginEffect(UniRandom random) => s_currentlyActive = true;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => Enabled.Value && base.CanBeginEffect(ctx);
    public override int GetEffectCost() => 1;

    [HarmonyPostfix, HarmonyPatch(typeof(StyleHUD), nameof(StyleHUD.GetLocalizedName))]
    public static void Patch(ref string __result)
    {
        if (!s_currentlyActive)
        {
            return;
        }

        int globalSeed = UltraTelephoneTwo.Instance.Random.Seed;
        Vector3 position = NewMovement.Instance.transform.position;
        int sceneSeed = UniRandom.StringToSeed(SceneHelper.CurrentScene);
        int seed = globalSeed ^ UniRandom.StringToSeed(position.ToString()) ^ sceneSeed;

        UniRandom rng = new UniRandom(seed);
        __result = rng.SelectRandomList(wordlist);
    }
}
