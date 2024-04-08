using System.Collections;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
public class GammaFluctuation : ChaosEffect
{
    [Configgable("Chaos/Effects", "Gamma Fluctuation")]
    private static ConfigToggle s_enabled = new(true);

    private Coroutine _lagRoutine;
    private UniRandom _random;
    private float _target;
    private static bool s_active;

    public override void BeginEffect(UniRandom random)
    {
        _random = random;
        _lagRoutine = StartCoroutine(Lag());
        s_active = true;
    }

    public override void Dispose()
    {
        StopCoroutine(_lagRoutine);
        base.Dispose();
    }

    private void Update()
    {
        if (!s_active)
        {
            return;
        }

        Shader.SetGlobalFloat("_Gamma", Mathf.MoveTowards(Shader.GetGlobalFloat("_Gamma"), _target, Time.deltaTime));
    }

    public override int GetEffectCost() => 2;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    private IEnumerator Lag()
    {
        //yield return new WaitForSecondsRealtime(_random.Range(10, 60));

        while (true)
        {
            for (int i = 0; i < _random.Range(3, 10); i++)
            {
                _target = _target < 1 ? _random.Range(1.5f, 2f) : _random.Range(0f, 0.5f);
                yield return new WaitForSecondsRealtime(_random.Range(2f, 5f));
            }

            yield return new WaitForSecondsRealtime(_random.Range(2f, 5f));
            _target = PrefsManager.Instance.GetFloat("gamma", 1);
            yield return new WaitForSecondsRealtime(_random.Range(20f, 45f));
        }
    }

    protected override void OnDestroy()
    {
        Shader.SetGlobalFloat("_Gamma", PrefsManager.Instance.GetFloat("gamma", 1));
        s_active = false;
    }
}
