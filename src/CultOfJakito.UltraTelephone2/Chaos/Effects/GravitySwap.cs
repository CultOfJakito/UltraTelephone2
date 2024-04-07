using System.Collections;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
public class GravitySwap : ChaosEffect
{
    [Configgable("Chaos/Effects", "Gravity Switcher")]
    private static ConfigToggle s_enabled = new(true);

    private Coroutine _lagRoutine;
    private UniRandom _random;
    private Vector3 _defaultGravity;

    public override void BeginEffect(UniRandom random)
    {
        _random = random;
        _lagRoutine = StartCoroutine(Lag());
        _defaultGravity = Physics.gravity;
    }

    public override void Dispose()
    {
        StopCoroutine(_lagRoutine);
        base.Dispose();
    }

    public override int GetEffectCost() => 2;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    private IEnumerator Lag()
    {
        yield return new WaitForSecondsRealtime(_random.Range(10, 60));

        while (true)
        {
            Physics.gravity = UnityEngine.Random.onUnitSphere * _random.Range(0.5f, 2);
            yield return new WaitForSecondsRealtime(_random.Range(3, 10));
            Physics.gravity = _defaultGravity;
            yield return new WaitForSecondsRealtime(_random.Range(20f, 45f));
        }
    }

    protected override void OnDestroy()
    {
        Physics.gravity = _defaultGravity;
        GameStateManager.Instance.EvaluateState();
    }
}
