using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
public class Ponder : ChaosEffect
{
    // Guys please add more prompts, my brain isn't made for that
    private static List<string> _prompts => UT2TextFiles.S_PonderPrompts.TextList;

    [Configgable("Chaos/Effects", "Enabled Pondering")]
    public static ConfigToggle Enabled = new(true);

    private UniRandom rng;
    public override void BeginEffect(UniRandom random)
    {
        rng = random;
        GameEvents.OnLevelStateChange += OnLevelStateChange;
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => Enabled.Value && base.CanBeginEffect(ctx);

    public override int GetEffectCost() => 1;


    public void OnLevelStateChange(LevelStateChangeEvent e)
    {
        if (e.IsPlaying)
        {
            HudMessageReceiver.Instance.SendHudMessage(rng.SelectRandomList(_prompts));
        }
    }

    protected override void OnDestroy() => GameEvents.OnLevelStateChange -= OnLevelStateChange;
}
