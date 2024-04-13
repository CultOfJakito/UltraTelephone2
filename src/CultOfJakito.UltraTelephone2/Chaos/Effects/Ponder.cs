using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using CultOfJakito.UltraTelephone2.Placeholders;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects;

[RegisterChaosEffect]
public class Ponder : ChaosEffect
{
    // Guys please add more prompts, my brain isn't made for that
    private static List<string> _prompts => UT2TextFiles.PonderPromptsFile.TextList;

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
            string message = rng.SelectRandom(_prompts);
            message = PlaceholderHelper.ReplacePlaceholders(message);
            HudMessageReceiver.Instance.SendHudMessage(message);
        }
    }

    protected override void OnDestroy() => GameEvents.OnLevelStateChange -= OnLevelStateChange;
}
