using BepInEx.Logging;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

internal class ChaosManager : MonoBehaviour, IDisposable {
	[Inject]
	public ManualLogSource Logger { get; set; }
	[Inject]
	public IEnumerable<IChaosEffect> ChaosEffects { get; set; }
	[Inject]
	public System.Random Random { get; set; }

	public void BeginEffects() {
		Logger.LogInfo("Chaos started");
		foreach(IChaosEffect effect in ChaosEffects) {
			effect.BeginEffect(new System.Random(Random.Next()));
		}
	}

	public void Dispose() {
		Destroy(this);
	}
}
