using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class CantRead : ChaosEffect
    {
        [Configgable("Chaos/Effects/Cant Read", "Cant Read")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/Cant Read", "Jumble Text Instead")]
        private static ConfigToggle s_jumbleTextInstead = new ConfigToggle(true);

        private static bool s_effectActive = false;

        private static List<string> s_illegebleWords => UT2TextFiles.S_CantReadWordsFile.TextList;

        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost()
        {
            return 2;
        }

        protected override void OnDestroy() => s_effectActive = false;


        [HarmonyPatch(typeof(ScanningStuff), nameof(ScanningStuff.ScanBook)), HarmonyPrefix]
        private static void OnScanBook(ScanningStuff __instance, ref string text)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            if (s_jumbleTextInstead.Value)
            {
                string copy = text;
                List<string> words = new List<string>(copy.Split(' '));

                for (int i = 0; i < words.Count; i++)
                {
                    string word = words[i];
                    char[] chars = s_rng.Shuffle(word.ToCharArray()).ToArray();
                    words[i] = new string(chars);
                }

                copy = string.Join(" ", words);
                text = copy;
            }
            else
            {
                int words = text.Split(' ').Length;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < words; i++)
                {
                    sb.Append(s_rng.SelectRandom(s_illegebleWords));
                    sb.Append(' ');
                }

                text = sb.ToString().TrimEnd(' ');
            }

            HudMessageReceiver.Instance.SendHudMessage("I should really learn how to read...");
        }
    }
}
