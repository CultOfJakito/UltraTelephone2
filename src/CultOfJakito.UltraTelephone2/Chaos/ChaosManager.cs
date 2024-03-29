using System.Reflection;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

public class ChaosManager : MonoBehaviour, IDisposable
{
    [Configgable(displayName:"Chaos Budget")]
    private static ConfigInputField<int> _chaosBudget = new ConfigInputField<int>(32, (v) =>
    {
        return v > 0;
    });

    public void BeginEffects()
    {
        //Seed is global and scene name to give a unique seed for each scene, while still being deterministic
        int seed = UltraTelephoneTwo.Instance.Random.Seed ^ UniRandom.StringToSeed(SceneHelper.CurrentScene);
        UniRandom random = new UniRandom(seed);
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
        activatedEffects = _ctx.GetCurrentSelection();

        foreach (IChaosEffect effect in activatedEffects)
        {
            Debug.Log($"Beginning Effect: {effect.GetType().Name}");
            effect.BeginEffect(new UniRandom(random.Next()));
        }
    }

    private List<IChaosEffect> activatedEffects;

    private bool _levelBegan;
    private ChaosSessionContext _ctx;

    private void Update()
    {
        if (_levelBegan)
        {
            if (InGameCheck.PlayingLevel())
                return;

            GameEvents.OnLevelStateChange.Invoke(new LevelStateChangeEvent(false, SceneHelper.CurrentScene));
            Dispose();
        }
        else
        {
            if (InGameCheck.PlayingLevel())
            {
                _levelBegan = true;

                GameEvents.OnLevelStateChange.Invoke(new LevelStateChangeEvent(true, SceneHelper.CurrentScene));
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

            IChaosEffect chaosEffectObject = null;

            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                chaosEffectObject = gameObject.AddComponent(type) as IChaosEffect;
            }
            else
            {
                chaosEffectObject = (IChaosEffect)Activator.CreateInstance(type);
            }

            chaosEffects.Add(chaosEffectObject);
        }

        return chaosEffects;
    }

    public void Dispose()
    {
        if(activatedEffects != null)
        {
            for (int i = 0; i < activatedEffects.Count; i++)
            {
                if (activatedEffects[i] == null)
                    continue;

                IChaosEffect effect = activatedEffects[i];
                activatedEffects[i] = null;
                effect.Dispose();
            }
        }

        Destroy(this);
    }
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
