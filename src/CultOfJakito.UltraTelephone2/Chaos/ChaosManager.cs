﻿using System.Reflection;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

public class ChaosManager : MonoBehaviour, IDisposable
{
    [Configgable(displayName:"Chaos Budget")]
    private static ConfigInputField<int> _chaosBudget = new ConfigInputField<int>(32, (v) =>
    {
        return v > 0;
    });

    [Configgable("Extras/Advanced", "Panic Button")]
    private static ConfigKeybind s_panicButton = new ConfigKeybind(KeyCode.P);

    [Configgable(displayName:"Chaos Enabled")]
    private static ConfigToggle s_enabled = new ConfigToggle(true);

    public static int ChaosBookHashCode { get; private set; }

    public void BeginEffects()
    {
        //Seed is global and scene name to give a unique seed for each scene, while still being deterministic
        int seed = new SeedBuilder()
            .WithGlobalSeed()
            .WithSceneName()
            .GetSeed();

        UniRandom random = new UniRandom(seed);
        int budget = _chaosBudget.Value;

        if (Crash.IsDestabilized)
        {
            budget = 600;
        }

        _ctx = new ChaosSessionContext(this, SceneHelper.CurrentScene, budget);

        foreach (IChaosEffect possibleEffect in GetChaosEffects().ShuffleIEnumerable(random))
        {
            if (_ctx.GetAvailableBudget() == 0)
            {
                break;
            }

            if (possibleEffect.CanBeginEffect(_ctx))
            {
                _ctx.Add(possibleEffect);
            }
            else
            {
                if(typeof(MonoBehaviour).IsAssignableFrom(possibleEffect.GetType()))
                {
                    //destroy the effect if it is a monobehaviour since it should not be used
                    Destroy(possibleEffect as MonoBehaviour);
                }
            }
        }

        Debug.Log("Chaos started");
        activatedEffects = _ctx.GetCurrentSelection();

        StringBuilder effectlistString = new();

        foreach (IChaosEffect effect in activatedEffects)
        {
            effectlistString.AppendLine(effect.GetType().Name);
            Debug.Log($"Beginning Effect: {effect.GetType().Name}");
            effect.BeginEffect(new UniRandom(random.Next()));
        }

        if(random.Chance(0.2f))
            DropChaosBook(effectlistString.ToString());
    }

    private void DropChaosBook(string bookText)
    {
        GameObject book = GameObject.Instantiate(UkPrefabs.Book.GetObject());
        Readable readable = book.GetComponent<Readable>();
        readable.content = bookText;
        ChaosBookHashCode = readable.gameObject.GetHashCode();

        book.transform.position = CameraController.Instance.transform.position;
    }

    private List<IChaosEffect> activatedEffects;

    public List<IChaosEffect> GetActiveEffects() => new(activatedEffects);

    private bool _levelBegan;
    private ChaosSessionContext _ctx;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && s_panicButton.WasPeformed())
        {
            Dispose();
            return;
        }

        if (_levelBegan)
        {
            if (InGameCheck.PlayingLevel())
                return;

            GameEvents.OnLevelStateChange?.Invoke(new LevelStateChangeEvent(false, SceneHelper.CurrentScene));
            Dispose();
        }
        else
        {
            if (InGameCheck.PlayingLevel())
            {
                _levelBegan = true;
                GameEvents.OnLevelStateChange?.Invoke(new LevelStateChangeEvent(true, SceneHelper.CurrentScene));
            }
        }
    }

    private IEnumerable<IChaosEffect> GetChaosEffects()
    {
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
        DisposeEffects();
        Destroy(this);
    }

    private void DisposeEffects()
    {
        if (activatedEffects != null)
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

        activatedEffects = null;
    }

    private void OnDestory()
    {
        DisposeEffects();
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

    public bool ContainsEffect<T>() where T : IChaosEffect => _selection.Any(x => x.GetType() == typeof(T));

    public void ClearSelection() => _selection.Clear();
}
