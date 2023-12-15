using System.Reflection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2;

internal sealed class PlayerItemChangedEventArgs : EventArgs {
	public ItemIdentifier Item { get; }

	public PlayerItemChangedEventArgs(ItemIdentifier item) {
		Item = item;
	}
}

internal static class EventBus {
	private static bool s_playerDiedRaised = false;
	public static event Action PlayerDied;
    public static event Action RestartedFromCheckpoint;
	public static event EventHandler<PlayerItemChangedEventArgs> PlayerItemChanged;

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


    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Restart))]
    static class RaiseRestartedFromCheckpointPatch {
        public static void Postfix(){
			RestartedFromCheckpoint?.Invoke();
        }
    }

	[HarmonyPatch]
	static class RaisePlayerItemChangedOnPlacePatch {
		public static IEnumerable<MethodInfo> TargetMethods() {
			yield return typeof(Punch).GetMethod(nameof(Punch.PlaceHeldObject));
			yield return typeof(Punch).GetMethod(nameof(Punch.ResetHeldState));
			yield return typeof(Punch).GetMethod(nameof(Punch.ForceThrow));
		}

		public static void Postfix() {
			PlayerItemChanged?.Invoke(null, new PlayerItemChangedEventArgs(null));
		}
	}

	[HarmonyPatch]
	static class RaisePlayerItemChangedOnGrabPatch {
		public static IEnumerable<MethodInfo> TargetMethods() {
			yield return typeof(Punch).GetMethod(nameof(Punch.ForceHold));
		}

		public static void Postfix(Punch __instance) {
			PlayerItemChanged?.Invoke(null, new PlayerItemChangedEventArgs(__instance.heldItem));
		}
	}
}
