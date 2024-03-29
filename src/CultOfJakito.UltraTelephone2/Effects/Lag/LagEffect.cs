using System.Collections;
using System.Runtime.InteropServices;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Effects.Lag;

[RegisterChaosEffect]
public class LagEffect : ChaosEffect
{
    [Configgable("Chaos Effects", "Lag")]
    private static ConfigToggle s_enabled = new(true);

    private Coroutine _lagRoutine;

    public override void BeginEffect(UniRandom random)
    {
        _lagRoutine = StartCoroutine(Lag());
    }

    public override void Dispose()
    {
        StopCoroutine(_lagRoutine);
    }

    public override int GetEffectCost() => 5;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

    private IEnumerator Lag()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(2.5f, 7.5f));

            float timer = 0;
            float timerLength = UnityEngine.Random.Range(0.25f, 0.5f);
            bool fasterOrSlower = UnityEngine.Random.value > 0.5f;

            while (timer < timerLength)
            {
                timer += Time.unscaledDeltaTime;

                if (Time.timeScale != 0)
                {
                    Time.timeScale = fasterOrSlower ? UnityEngine.Random.Range(0.25f, 0.5f) : UnityEngine.Random.Range(1.5f, 2f);
                }

                yield return null;
            }

            Time.timeScale = 1;
            yield return null;
        }
    }
}
