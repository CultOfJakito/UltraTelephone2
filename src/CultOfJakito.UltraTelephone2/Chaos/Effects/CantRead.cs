using System.Text;
using Configgy;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;

namespace CultOfJakito.UltraTelephone2.Chaos
{
    [HarmonyPatch]
    [RegisterChaosEffect]
    public class CantRead : ChaosEffect
    {
        [Configgable("Chaos/Effects/Cant Read", "Cant Read")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static bool s_effectActive = false;

        private static List<string> s_illegebleWords => UT2TextFiles.S_CantReadWordsFile.TextList;

        private static UniRandom s_rng;

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            s_effectActive = true;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        public override int GetEffectCost() => 1;

        protected override void OnDestroy() => s_effectActive = false;


        [HarmonyPatch(typeof(ScanningStuff), nameof(ScanningStuff.ScanBook)), HarmonyPrefix]
        private static void OnScanBook(ScanningStuff __instance, ref string text)
        {
            if (!s_enabled.Value || !s_effectActive)
                return;

            int option = s_rng.Next(0, 3);

            //0 = Write console log to book
            if (option == 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach(string logMessage in UltraTelephoneTwo.LogBuffer)
                {
                    sb.AppendLine(logMessage);
                }

                text = sb.ToString();

            } else if (option == 1) //1 = Jumble letters
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
            else if(option == 3) //3 - replace every word with a random word
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
            else //4 - replace every character with :3 or ;3
            {
                int charCount = text.Length;

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < charCount; i++)
                {
                    sb.Append((s_rng.Bool()) ? ":3" : ";3");

                    if (i + 1 < charCount)
                        sb.Append(' ');
                }

                text = sb.ToString();
            }
            

            HudMessageReceiver.Instance.SendHudMessage("I should really learn how to read...");
        }
    }
}
