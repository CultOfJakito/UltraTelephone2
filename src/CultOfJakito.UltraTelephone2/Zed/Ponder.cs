using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ULTRAKILL;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2.Chaos
{
    [RegisterChaosEffect]
    public class Ponder : ChaosEffect, ILevelEvents
    {
        // Guys please add more prompts, my brain isn't made for that
        private List<string> Prompts =
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
        public static ConfigToggle Enabled = new ConfigToggle(true);
        public override void BeginEffect(System.Random random)
        {
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if (!Enabled.Value)
                return false;

            return base.CanBeginEffect(ctx);
        }

        public override int GetEffectCost()
        {
            return 1;
        }

        public void OnLevelStarted(string levelName)
        {
            HudMessageReceiver.Instance.SendHudMessage(Prompts[UnityEngine.Random.Range(0, Prompts.Count - 1)]);
        }

        public void OnLevelComplete(string levelName) { }
    }
}
