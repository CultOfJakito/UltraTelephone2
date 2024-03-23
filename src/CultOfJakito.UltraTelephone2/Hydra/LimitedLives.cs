using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Hydra
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class LimitedLives : ChaosEffect, IEventListener
    {
        [Configgable("Hydra/Chaos", "Limited Lives")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Hydra/Chaos", "Limited Life Count")]
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
            return 1;
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
                        OnClick = () =>
                        {
                            if(livesLeft > 0)
                                return;

                            SceneHelper.LoadScene("Main Menu");
                        }
                    }
                }
            });
        }

        public override void Dispose()
        {
            GameEvents.OnPlayerDeath -= OnPlayerDeath;
            base.Dispose();
        }
    }
}
