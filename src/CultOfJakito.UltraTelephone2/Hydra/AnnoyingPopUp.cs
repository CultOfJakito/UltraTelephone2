using System;
using System.Collections.Generic;
using System.Text;
using Configgy;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos
{
    internal class AnnoyingPopUp : ChaosEffect
    {
        [Configgable("Hydra","Show Annoying Death Messages")]
        private static ConfigToggle showAnnoyingPopUps = new ConfigToggle(true);

        private System.Random rng;

        public override void BeginEffect(System.Random random)
        {
            rng = random;
            dialogueEvent = new ModalDialogueEvent();
            EventBus.RestartedFromCheckpoint += ShowPopUp;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            bool canBegin = base.CanBeginEffect(ctx);

            if (!canBegin)
                return false;

            return showAnnoyingPopUps.Value;
        }

        private void ShowPopUp()
        {
            RandomizeEvent();
            ModalDialogue.ShowDialogue(dialogueEvent);
        }

        private static readonly string[] titles =
        {
            "You died!",
            "You died again!",
            "You died for the 100th time!",
            "You died for the 1000th time!",
            "You suck!",
            "You suck at this game!",
            "Please Improve",
            "Uninstall?",
            "You're bad",
            "You're bad at this game",
            "You're bad at this game and you should feel bad",
            "You're bad at this game and you should feel bad and you should uninstall",
            "You're bad at this game and you should feel bad and you should uninstall and you should never play again",
            "It's okay!"
        };

        private static readonly string[] messages =
        {
            "When you play this game, you are supposed to not die.",
            "Dying is bad. Stop doing that.",
            "If you die, you will not win.",
            "If you die, you will not get the good ending.",
            "If you die, you will not get the secret ending.",
            "If you die, you will not get the true ending.",
            "If you die, you will not get the secret true ending.",
            "Why are you bad",
            "Please improve",
            "We all make mistakes.",
            "You're not a mistake, but you're making mistakes.",
            "You're not a mistake, but you're making mistakes. Please improve.",
        };

        private static readonly string[] optionNames =
        {
            "Yes",
            "No",
            "Maybe",
            "I don't know",
            "Can you repeat the question?",
            "You're not the boss of me now",
            "You're the boss of me now",
            "Life is unfair",
            "I'm not gonna be just a face in the crowd",
            "I'm gonna hear my voice when I shout it out loud",
            "It's my life",
            "It's now or never",
            "Confirm",
            "Cancel",
            "Ok",
            "I'm sorry",
            "I'm not sorry",
            "I'm sorry, not sorry",
            "Really?!",
            "Fuck off",
            "Don't Show Again",
            "Show Again",
            "NO!!!",
            "Kick Rocks",
            "Improve",
            "Click Here!!",
            "BUTTON_TEXT",
            "NULL"
        };

        private static readonly Color orange = new Color(1, 0.682f, 0, 1f);

        private ModalDialogueEvent dialogueEvent;

        private void RandomizeEvent()
        {
            dialogueEvent.Title = titles.RandomElement(rng);
            dialogueEvent.Message = messages.RandomElement(rng);

            dialogueEvent.Options = new DialogueBoxOption[rng.Next(1,4)];

            for(int i =0; i < dialogueEvent.Options.Length; i++)
            {
                dialogueEvent.Options[i] = CreateRandomOption();
            }
        }

        private DialogueBoxOption CreateRandomOption()
        {
            return new DialogueBoxOption()
            {
                Name = optionNames.RandomElement(rng),
                Color = orange,
                OnClick = () =>
                {
                    if(rng.Next(1,4) == 1)
                        ShowPopUp();
                }
            };
        }

        public override int GetEffectCost()
        {
            return 1;
        }

        private void OnDestroy()
        {
            EventBus.RestartedFromCheckpoint -= ShowPopUp;
        }
    }
}
