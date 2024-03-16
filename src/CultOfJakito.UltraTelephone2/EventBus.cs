using HarmonyLib;

namespace CultOfJakito.UltraTelephone2;

internal static class EventBus
{
    private static bool s_playerDiedRaised;
    public static event Action PlayerDied;

    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHurt))]
    private static class RaisePlayerDiedPatch
    {
        public static void Postfix(NewMovement __instance)
        {
            if (!__instance.dead || s_playerDiedRaised)
            {
                return;
            }

            s_playerDiedRaised = true;
            PlayerDied?.Invoke();
        }
    }

    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.ActivatePlayer))]
    private static class SetPlayerDiedRaisedFalsePatch
    {
        public static void Postfix() => s_playerDiedRaised = false;
    }

    public static event Action<bool> RestartedFromCheckpoint;

    [HarmonyPatch]
    private static class RaiseRestartedFromCheckpointPatch
    {
        private static bool s_died;

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Restart))]
        [HarmonyPostfix]
        public static void Postfix()
        {
            RestartedFromCheckpoint?.Invoke(s_died);
            s_died = false;
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Restart))]
        [HarmonyPrefix]
        public static void Prefix() => s_died = NewMovement.Instance.hp <= 0;
    }
}
