﻿using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[HarmonyPatch]
[RegisterChaosEffect]
public class IdolsForEveryone : ChaosEffect
{
    [Configgable("Chaos/Effects", "Idols For Everyone!")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;
    private UniRandom _random;

    public override void BeginEffect(UniRandom random)
    {
        _random = random;
        s_effectActive = true;
    }


    [HarmonyPatch(typeof(EnemyIdentifier), "Awake")]
    [HarmonyPostfix]
    public static void OnEnemySpawn(EnemyIdentifier __instance)
    {
        if (!s_enabled.Value || !s_effectActive)
        {
            return;
        }

        if (__instance.dead)
        {
            return;
        }

        //Dont spawn idols for idols or centaur ffs
        if (__instance.enemyType is EnemyType.Idol or EnemyType.Centaur)
        {
            return;
        }

        Vector3 pos = __instance.transform.position;
        Transform parent = __instance.transform.parent;

        if(__instance.enemyType == EnemyType.V2 && SceneHelper.CurrentScene == "Level 1-4")
        {
            pos = new Vector3(0, -21.4022f, 625.0509f);
        }

        if (parent == null)
        {
            return;
        }

        //spawn an idol
        UkPrefabs.Idol.LoadObjectAsync((s, r) =>
        {
            if (__instance == null || parent == null || s != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                return;
            }

            GameObject newIdol = Instantiate(r, pos, Quaternion.identity);
            newIdol.transform.SetParent(parent, true);
            Idol idol = newIdol.GetComponent<Idol>();
            idol.ChangeOverrideTarget(__instance);
        });
    }

    //this effect is hell. 10 cost.
    public override int GetEffectCost() => 10;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    protected override void OnDestroy() => s_effectActive = false;
}
