using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class LimitedLives : ChaosEffect 
    {
        [Configgable("Chaos/Effects/LimitedLives", "Limited Lives")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/LimitedLives", "Limited Life Count")]
        private static ConfigInputField<int> s_lives = new ConfigInputField<int>(3, (v) =>
        {
            return v > 0;
        });

        public override void BeginEffect(UniRandom random)
        {
            livesLeft = s_lives.Value;
            GameEvents.OnPlayerDeath += OnPlayerDeath;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 3;
        }

        private int livesLeft;

        private void OnPlayerDeath()
        {
            --livesLeft;

            string message = livesLeft > 0 ? $"You have {livesLeft} lives left." : "You have no lives left.";
            string button = livesLeft > 0 ? "Ok." : "Main Menu";

            ModalDialogue.ShowDialogue(new ModalDialogueEvent()
            {
                Title = "You Died.",
                Message = message,
                Options = new DialogueBoxOption[]
                {
                    new DialogueBoxOption()
                    {
                        Name = button,
                        Color = Color.red,
                        OnClick = () =>
                        {
                            if(livesLeft > 0)
                                return;

                            SceneHelper.LoadScene("Main Menu");
                            livesLeft = s_lives.Value;
                        }
                    }
                }
            });
        }

        protected override void OnDestroy()
        {
            GameEvents.OnPlayerDeath -= OnPlayerDeath;
            livesLeft = s_lives.Value;
        }

    }
}
