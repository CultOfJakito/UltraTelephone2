using System.Reflection;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

public class ChaosManager : MonoBehaviour, IDisposable
{ 

	public void BeginEffects()
    {
        System.Random random = UltraTelephone2.UltraTelephoneTwo.Instance.Random;
		ctx = new(this, SceneHelper.CurrentScene, 32);

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

    private bool levelBegan;
    private ChaosSessionContext ctx;

    private void Update()
    {
        if (levelBegan)
        {
            if (!InGameCheck.PlayingLevel())
            {
                foreach (var x in ctx.GetCurrentSelection())
                {
                    if (typeof(ILevelEvents).IsAssignableFrom(x.GetType()))
                    {
                        ((ILevelEvents)x).OnLevelComplete(ctx.LevelName);
                    }
                }
            }
        }
        else
        {
            if (InGameCheck.PlayingLevel())
            {
                levelBegan = true;
                foreach (var x in ctx.GetCurrentSelection())
                {
                    if (typeof(ILevelEvents).IsAssignableFrom(x.GetType()))
                    {
                        ((ILevelEvents)x).OnLevelStarted(ctx.LevelName);
                    }
                }
            }
        }
    }

    private IEnumerable<IChaosEffect> GetChaosEffects()
    {
        //TODO this fails to clean up monobehaviour types
        List<IChaosEffect> chaosEffects = new List<IChaosEffect>();

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsAbstract)
                continue;

            if (type.GetCustomAttribute<RegisterChaosEffectAttribute>() == null)
                continue;

            if (!typeof(IChaosEffect).IsAssignableFrom(type))
                continue;

           
            if(typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                chaosEffects.Add(gameObject.AddComponent(type) as IChaosEffect);
            }
            else
            {
                chaosEffects.Add((IChaosEffect)Activator.CreateInstance(type));
            }
        }

        return chaosEffects;
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
