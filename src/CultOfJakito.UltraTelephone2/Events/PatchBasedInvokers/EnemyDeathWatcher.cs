using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Events.PatchBasedInvokers
{
    [HarmonyPatch]
    public static class EnemyDeathWatcherPatch
    {
        [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Awake)), HarmonyPostfix]
        private static void OnEnemySpawned(EnemyIdentifier __instance)
        {
            if(!__instance.TryGetComponent<EnemyDeathWatcher>(out EnemyDeathWatcher watcher))
                __instance.gameObject.AddComponent<EnemyDeathWatcher>();
        }
    }

    public class EnemyDeathWatcher : MonoBehaviour
    {
        private EnemyIdentifier eid;
        private bool died;

        private void Awake()
        {
            eid = GetComponent<EnemyIdentifier>();
            eid.onDeath.AddListener(EnemyDied);
        }

        private void EnemyDied()
        {
            if (died)
                return;

            died = true;
            GameEvents.OnEnemyDeath?.Invoke(new EnemyDeathEvent(eid, eid.hitter));
        }
    }
}
