using System.Collections;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos.Effects.MovingWindow;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
public class ResolutionSwitcher : ChaosEffect
{
    [Configgable("Chaos/Effects", "Resolution Switcher")]
    private static ConfigToggle s_enabled = new(true);

    private Coroutine _lagRoutine;
    private UniRandom _random;
    private float _target;
    private Vector2Int _startRes;
    private FullScreenMode _startMode;

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

    private void Update()
    {
        _target = Mathf.MoveTowards(Shader.GetGlobalFloat("_Gamma"), _target, Time.deltaTime);
        Shader.SetGlobalFloat("_Gamma", _target);
    }

    public override int GetEffectCost() => 4;
    public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx) && !ctx.ContainsEffect<WindowDanceEffect>();

    private IEnumerator Lag()
    {
        yield return new WaitForSecondsRealtime(_random.Range(10, 60));

        while (true)
        {
            for (int i = 0; i < _random.Range(1, 5); i++)
            {
                Vector2Int oldRes = ResolutionFuckeryUtils.StandardResolution;
                Screen.SetResolution(UnityEngine.Random.Range(oldRes.x / 2, oldRes.x), UnityEngine.Random.Range(oldRes.y / 2, oldRes.y), FullScreenMode.ExclusiveFullScreen);
                yield return new WaitForSecondsRealtime(_random.Range(2f, 5f));
            }

            yield return new WaitForSecondsRealtime(_random.Range(2f, 5f));
            ResolutionFuckeryUtils.ResetToDefault();
            yield return new WaitForSecondsRealtime(_random.Range(20f, 45f));
        }
    }

    protected override void OnDestroy()
    {
        ResolutionFuckeryUtils.ResetToDefault();
    }
}
