using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class NoEscape : ChaosEffect
    {
        [Configgable("Hydra/Chaos", "No Escape")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        public override void BeginEffect(System.Random random)
        {
            DisableEscapeButtons();
        }

        private void DisableEscapeButtons()
        {
            Transform pauseMenuTF = CanvasController.Instance.transform.Find("PauseMenu");
            if (pauseMenuTF == null)
                return;

            pauseMenuTF.transform.GetComponentsInChildren<Button>().Where(x=>x.name == "Quit Mission").FirstOrDefault()?.gameObject.SetActive(false);
            pauseMenuTF.transform.GetComponentsInChildren<Button>().Where(x=>x.name == "Restart Mission").FirstOrDefault()?.gameObject.SetActive(false);
            pauseMenuTF.transform.GetComponentsInChildren<Button>().Where(x=>x.name == "Restart Checkpoint").FirstOrDefault()?.gameObject.SetActive(false);
        }

        public override int GetEffectCost()
        {
            return 4;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if (!s_enabled.Value)
                return false;

            switch (ctx.LevelName)
            {
                case "uk_construct":
                    return false;
                case "CreditsMuseum2":
                    return false;
                case "Endless":
                    return false;
            }

            return base.CanBeginEffect(ctx);
        }

    }
}
