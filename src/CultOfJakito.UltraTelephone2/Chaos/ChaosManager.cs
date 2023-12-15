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

		ChaosSessionContext ctx = new(this, SceneHelper.CurrentScene, 32);

        foreach (IChaosEffect possibleEffect in ChaosEffects.Shuffle(Random))
		{
			if (ctx.GetAvailableBudget() == 0)
				break;

			if (possibleEffect.CanBeginEffect(ctx))
				ctx.Add(possibleEffect);
		}

		Logger.LogInfo("Chaos started");
		foreach(IChaosEffect effect in ctx.GetCurrentSelection()) {
			effect.BeginEffect(new System.Random(Random.Next()));
		}
	}

	public void Dispose() {
		Destroy(this);
	}
}

internal class ChaosSessionContext
{
	public ChaosManager ChaosManager { get; }
	public string LevelName { get; }
	public int TotalBudget { get; }

	private List<IChaosEffect> selection;

	public ChaosSessionContext(ChaosManager chaosManager, string levelName, int budget)
	{
        ChaosManager = chaosManager;
		this.LevelName = levelName;
		this.TotalBudget = budget;
		selection = new List<IChaosEffect>();
    }

	public int GetAvailableBudget()
	{
		return TotalBudget - selection.Sum(x=>x.GetEffectCost());
	}

	public void Add(IChaosEffect effect)
	{
		selection.Add(effect);
	}

	public List<IChaosEffect> GetCurrentSelection()
	{
		return new List<IChaosEffect>(selection);
    }

	public void ClearSelection()
	{
		selection.Clear();
	}
}
