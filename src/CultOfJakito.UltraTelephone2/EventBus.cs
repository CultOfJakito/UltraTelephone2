using HarmonyLib;

namespace CultOfJakito.UltraTelephone2;

internal static class EventBus {
	public static event Action PlayerDied;

	[HarmonyPatch(typeof(NewMovement), nameof(NewMovement.GetHurt))]
	static class RaisePlayerDiedPatch {
		private static bool s_playerDiedRaised = false;
		[HarmonyPostfix]
		public static void Postfix(NewMovement __instance) {
			if(!__instance.dead) {
				s_playerDiedRaised = false;
			} else if(!s_playerDiedRaised) {
				s_playerDiedRaised = true;
				PlayerDied?.Invoke();
			}
		}
	}
}
