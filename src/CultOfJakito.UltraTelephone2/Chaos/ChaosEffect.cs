using UnityEngine;
namespace CultOfJakito.UltraTelephone2.Chaos;

public abstract class ChaosEffect : MonoBehaviour, IChaosEffect, IDisposable
{
    public abstract void BeginEffect(UniRandom random);

    public virtual bool CanBeginEffect(ChaosSessionContext ctx) => ctx.GetAvailableBudget() >= GetEffectCost();

    public abstract int GetEffectCost();

    public virtual void Dispose() => Destroy(this);

    protected abstract void OnDestroy();
}
