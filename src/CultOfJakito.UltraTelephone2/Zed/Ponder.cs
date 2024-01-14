using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;

namespace CultOfJakito.UltraTelephone2.Zed;

[RegisterChaosEffect]
public class Ponder : ChaosEffect, ILevelEvents
{
    // Guys please add more prompts, my brain isn't made for that
    private List<string> _prompts =
    [
        "If your legs get cut off, would it hurt?",
        "101110010000101101011101101010110",
        "Weeeeeeeeeeeeeeeee",
        "How the fuck did a 1000-THR get in hell",
        "I thought a cerberus was a type of dog...",
        "What exactly generates the power from blood?",
        "Do guttermen dream?",
        "If Sisyphus could just break out of the prison, why did he wait?",
        "I just [EXCEPTION][EXCEPTION][EXCEPTION] Mindflayer [EXCEPTION]",
        "What if Guttertanks were always meant to topple?",
        "What if he made it up?",
        "What the heck them blue orbs",
        "What does P even stand for? Points?",
        "Who put Jakito behind bars?"
    ];

    [Configgable("ZedDev", "Enable pondering")]
    public static ConfigToggle Enabled = new(true);

    public override void BeginEffect(Random random)
    {
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => Enabled.Value && base.CanBeginEffect(ctx);

    public override int GetEffectCost() => 1;

    public void OnLevelStarted(string levelName) => HudMessageReceiver.Instance.SendHudMessage(_prompts[UnityEngine.Random.Range(0, _prompts.Count - 1)]);

    public void OnLevelComplete(string levelName)
    {
    }
}
