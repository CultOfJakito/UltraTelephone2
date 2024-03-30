using System.Collections;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.Lag;

[RegisterChaosEffect]
public class LagEffect : ChaosEffect
{
    [Configgable("Chaos/Effects", "Lag")]
    private static ConfigToggle s_enabled = new(true);

    private Coroutine _lagRoutine;
    private UniRandom _random;

    public override void BeginEffect(UniRandom random)
    {
        _random = random;
        _lagRoutine = StartCoroutine(Lag());
    }

    public override void Dispose()
    {
        StopCoroutine(_lagRoutine);
        base.Dispose();
    }

    public override int GetEffectCost() => 5;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    private IEnumerator Lag()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(_random.Range(2.5f, 7.5f));

            float timer = 0;
            float timerLength = _random.Range(0.25f, 0.5f);
            bool fasterOrSlower = _random.Chance(0.5f);

            while (timer < timerLength)
            {
                timer += Time.unscaledDeltaTime;

                if (Time.timeScale != 0)
                {
                    Time.timeScale = fasterOrSlower ? _random.Range(0.25f, 0.5f) : _random.Range(1.5f, 2f);
                }

                yield return null;
            }

            Time.timeScale = 1;
            yield return null;
        }
    }
}
