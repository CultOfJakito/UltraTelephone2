using System;
using System.Collections.Generic;
using System.Text;
using ULTRAKILL;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using Configgy;

namespace CultOfJakito.UltraTelephone2.Zed
{
    [RegisterChaosEffect]
    public class Ponder : ChaosEffect
    {
        public List<string> wtf =
        [
            "If your legs get cut off, would it hurt?",
            "101110010000101101011101101010110",
            "Weeeeeeeeeeeeeeeee",
            "How the fuck did a 1000-THR get in hell"
        ];
        [Configgable("ZedDev", "Enable pondering")]
        public static ConfigToggle Enabled = new ConfigToggle(true);
        public override void BeginEffect(Random random)
        {
            if (!Enabled.Value) return;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (a,b) =>
            {
                if(SceneHelper.CurrentScene.Contains("Level"))
                {
                    HudMessageReceiver.Instance.SendHudMessage(wtf[UnityEngine.Random.Range(0,wtf.Count-1)]));
                }
            };
        }
        public override bool CanBeginEffect(ChaosSessionContext ctx) => base.CanBeginEffect(ctx);
        public override int GetEffectCost()
        {
            return 0;
        }
    }
}
