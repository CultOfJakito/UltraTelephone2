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
    public class Ponder : ChaosEffect
    {
// Guys please add more prompts, my brain isn't made for that
private List<string> Prompts =
[
    "If your legs get cut off, would it hurt?",
    "101110010000101101011101101010110",
    "Weeeeeeeeeeeeeeeee",
    "How the fuck did a 1000-THR get in hell"
];
        [Configgable("ZedDev", "Enable pondering")]
        public static ConfigToggle Enabled = new ConfigToggle(true);
        private bool Subscribed = false;
        public override void BeginEffect(System.Random random)
        {
            if(Subscribed == false)
            {
                SceneManager.sceneLoaded += Handle;
                Subscribed = true;
            }
        }
        void OnDestroy()
        {
            SceneManager.sceneLoaded -= Handle;
            Subscribed = false;
        }
        private void Handle(Scene scene, LoadSceneMode mode)
        {
            if(SceneHelper.CurrentScene.Contains("Level"))
            {
                HudMessageReceiver.Instance.SendHudMessage(Prompts[UnityEngine.Random.Range(0,Prompts.Count-1)]);
            }
        }
        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            if(!Enabled.Value) return false;
            else return true;
        }
        public override int GetEffectCost()
        {
            return 0;
        }
    }
}
