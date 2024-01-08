using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2;

internal static class EventBus {
	private static bool s_playerDiedRaised = false;
	public static event Action PlayerDied;

	[HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHurt))]
	static class RaisePlayerDiedPatch {
		public static void Postfix(NewMovement __instance) {
			if(__instance.dead && !s_playerDiedRaised) {
				s_playerDiedRaised = true;
				PlayerDied?.Invoke();
			}
		}
	}

	[HarmonyPatch(typeof(NewMovement), nameof(NewMovement.ActivatePlayer))]
	static class SetPlayerDiedRaisedFalsePatch {
		public static void Postfix() {
			s_playerDiedRaised = false;
		}
	}

    public static event Action RestartedFromCheckpoint;

    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Restart))]
    static class RaiseRestartedFromCheckpointPatch {

        private static float timeOfLastRestart = 0f;
        public static void Postfix(){
            if(Time.realtimeSinceStartup - timeOfLastRestart < 0.01f)
                return;

            timeOfLastRestart = Time.realtimeSinceStartup;
			RestartedFromCheckpoint?.Invoke();
        }
    }
}
