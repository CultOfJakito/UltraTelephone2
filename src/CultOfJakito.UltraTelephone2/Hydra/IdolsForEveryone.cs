using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra;

[HarmonyPatch]
[RegisterChaosEffect]
public class IdolsForEveryone : ChaosEffect
{
    [Configgable("Hydra/Chaos", "Idols For All!")]
    private static ConfigToggle s_enabled = new(true);

    private static bool s_effectActive;
    private System.Random _random;

    public override void BeginEffect(System.Random random)
    {
        _random = random;
        s_effectActive = true;
    }


    [HarmonyPatch(typeof(EnemyIdentifier), "Awake")]
    [HarmonyPostfix]
    public static void OnEnemySpawn(EnemyIdentifier instance)
    {
        if (!s_enabled.Value || !s_effectActive)
        {
            return;
        }

        if (instance.dead)
        {
            return;
        }

        //Dont spawn idols for idols or centaur ffs
        if (instance.enemyType is EnemyType.Idol or EnemyType.Centaur)
        {
            return;
        }

        Vector3 pos = instance.transform.position;
        Transform parent = instance.transform.parent;

        if (parent == null)
        {
            return;
        }

        //spawn an idol
        UkPrefabs.Idol.LoadObjectAsync((s, r) =>
        {
            if (instance == null || parent == null || s != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                return;
            }

            GameObject newIdol = Instantiate(r, pos, Quaternion.identity);
            newIdol.transform.SetParent(parent, true);
            Idol idol = newIdol.GetComponent<Idol>();
            idol.ChangeOverrideTarget(instance);
        });
    }

    public override int GetEffectCost() => 5;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    private void OnDestroy() => s_effectActive = false;
}
