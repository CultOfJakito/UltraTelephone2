using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Hydra;

[RegisterChaosEffect]
[HarmonyPatch]
public class NoEscape : ChaosEffect
{
    [Configgable("Hydra/Chaos", "No Escape")]
    private static ConfigToggle s_enabled = new(true);

    public override void BeginEffect(UniRandom random) => DisableEscapeButtons();

    private void DisableEscapeButtons()
    {
        Transform pauseMenuTf = CanvasController.Instance.transform.Find("PauseMenu");
        if (pauseMenuTf == null)
        {
            return;
        }

        pauseMenuTf.transform.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "Quit Mission")?.gameObject.SetActive(false);
        pauseMenuTf.transform.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "Restart Mission")?.gameObject.SetActive(false);
        pauseMenuTf.transform.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "Restart Checkpoint")?.gameObject.SetActive(false);
    }

    public override int GetEffectCost() => 4;

    public override bool CanBeginEffect(ChaosSessionContext ctx)
    {
        if (!s_enabled.Value)
        {
            return false;
        }

        return ctx.LevelName switch
        {
            "uk_construct" => false,
            "CreditsMuseum2" => false,
            "Endless" => false,
            _ => base.CanBeginEffect(ctx)
        };
    }
}
