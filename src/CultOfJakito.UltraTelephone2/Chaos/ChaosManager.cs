using System.Reflection;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

public class ChaosManager : MonoBehaviour, IDisposable
{ 

	public void BeginEffects() {

        System.Random random = UltraTelephone2.UltraTelephoneTwo.Instance.Random;
		ChaosSessionContext ctx = new(this, SceneHelper.CurrentScene, 32);

        foreach (IChaosEffect possibleEffect in GetChaosEffects().Shuffle(random))
		{
			if (ctx.GetAvailableBudget() == 0)
				break;

			if (possibleEffect.CanBeginEffect(ctx))
				ctx.Add(possibleEffect);
		}

		Debug.Log("Chaos started");

		foreach(IChaosEffect effect in ctx.GetCurrentSelection()) {
			effect.BeginEffect(new System.Random(random.Next()));
		}
	}

    private IEnumerable<IChaosEffect> GetChaosEffects()
    {
        //TODO this fails to clean up monobehaviour types
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.GetCustomAttribute<RegisterChaosEffectAttribute>() != null && typeof(IChaosEffect).IsAssignableFrom(x) && !x.IsAbstract)
            .Select(x => (IChaosEffect)Activator.CreateInstance(x));
    }

	public void Dispose() {
		Destroy(this);
	}
}

public class ChaosSessionContext
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
