using System.Reflection;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

public class ChaosManager : MonoBehaviour, IDisposable
{
    public void BeginEffects()
    {
        System.Random random = UltraTelephoneTwo.Instance.Random;
        _ctx = new ChaosSessionContext(this, SceneHelper.CurrentScene, 32);

        foreach (IChaosEffect possibleEffect in GetChaosEffects().Shuffle(random))
        {
            if (_ctx.GetAvailableBudget() == 0)
            {
                break;
            }

            if (possibleEffect.CanBeginEffect(_ctx))
            {
                _ctx.Add(possibleEffect);
            }
        }

        Debug.Log("Chaos started");

        foreach (IChaosEffect effect in _ctx.GetCurrentSelection())
        {
            Debug.Log($"Beginning Effect: {effect.GetType().Name}");
            effect.BeginEffect(new System.Random(random.Next()));
        }
    }

    private bool _levelBegan;
    private ChaosSessionContext _ctx;

    private void Update()
    {
        if (_levelBegan)
        {
            if (InGameCheck.PlayingLevel())
            {
                return;
            }

            foreach (IChaosEffect x in _ctx.GetCurrentSelection())
            {
                if (typeof(ILevelEvents).IsAssignableFrom(x.GetType()))
                {
                    ((ILevelEvents)x).OnLevelComplete(_ctx.LevelName);
                }
            }
        }
        else
        {
            if (InGameCheck.PlayingLevel())
            {
                _levelBegan = true;
                foreach (IChaosEffect x in _ctx.GetCurrentSelection())
                {
                    if (typeof(ILevelEvents).IsAssignableFrom(x.GetType()))
                    {
                        ((ILevelEvents)x).OnLevelStarted(_ctx.LevelName);
                    }
                }
            }
        }
    }

    private IEnumerable<IChaosEffect> GetChaosEffects()
    {
        //TODO this fails to clean up monobehaviour types
        List<IChaosEffect> chaosEffects = new();

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsAbstract)
            {
                continue;
            }

            if (type.GetCustomAttribute<RegisterChaosEffectAttribute>() == null)
            {
                continue;
            }

            if (!typeof(IChaosEffect).IsAssignableFrom(type))
            {
                continue;
            }


            if (typeof(MonoBehaviour).IsAssignableFrom(type))
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

    public void Dispose() => Destroy(this);
}

public class ChaosSessionContext
{
    public ChaosManager ChaosManager { get; }
    public string LevelName { get; }
    public int TotalBudget { get; }

    private List<IChaosEffect> _selection;

    public ChaosSessionContext(ChaosManager chaosManager, string levelName, int budget)
    {
        ChaosManager = chaosManager;
        LevelName = levelName;
        TotalBudget = budget;
        _selection = new List<IChaosEffect>();
    }

    public int GetAvailableBudget() => TotalBudget - _selection.Sum(x => x.GetEffectCost());

    public void Add(IChaosEffect effect) => _selection.Add(effect);

    public List<IChaosEffect> GetCurrentSelection() => new(_selection);

    public void ClearSelection() => _selection.Clear();
}
