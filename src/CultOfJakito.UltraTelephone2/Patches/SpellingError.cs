using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using TMPro;

namespace CultOfJakito.UltraTelephone2.Patches
{
    [HarmonyPatch]
    public static class SpellingError
    {
        //[Configgable("Patches", "Spelling Error")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        //[HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.OnEnable)), HarmonyPostfix]
        private static void OnEnable(TextMeshProUGUI __instance)
        {
            if (!s_enabled.Value)
                return;

            UniRandom random = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSceneName().WithSeed(__instance.name));

            if (random.Chance(0.01f))
            {
                string text = __instance.text;
                __instance.text = MakeErrors(text, random);
            }
        }

        private static string MakeErrors(string text, UniRandom random)
        {
            for (int i = 0; i < text.Length; i++)
            {
                int choice = random.Range(0, 3);

                if (choice == 0)
                {
                    //double the letter
                    text = text.Insert(i, text[i].ToString());
                }else if (choice == 1)
                {
                    //remove the letter
                    text = text.Remove(i, 1);
                }
                else if (choice == 2)
                {
                    //swap the letter
                    int swapIndex = random.Range(0, text.Length);
                    char temp = text[i];
                    text = text.Remove(i, 1);
                    text = text.Insert(i, text[swapIndex].ToString());
                    text = text.Remove(swapIndex, 1);
                    text = text.Insert(swapIndex, temp.ToString());
                }

                if (text[i] == '!')
                {
                    if (random.Chance(0.2f))
                    {
                        text = text.Remove(i);
                        text = text.Insert(i, "?");
                    }

                }else if (text[i] == '?')
                {
                    if (random.Chance(0.2f))
                    {
                        text = text.Remove(i);
                        text = text.Insert(i, "!");
                    }
                }


                //Detect sentence ending and add a funny appendage
                if (text[i] == '.' && i+1 < text.Length && text[i+1] == ' ')
                {
                    if (random.Chance(0.2f))
                    {
                        int funnyAppend = random.Range(0, 6);

                        if (funnyAppend == 0)
                        {
                            text = text.Insert(i + 1, " Perchance?");
                        }
                        else if (funnyAppend == 1)
                        {
                            text = text.Insert(i + 1, " Allegedly of course.");
                        }
                        else if (funnyAppend == 2)
                        {
                            text = text.Insert(i + 1, " Or so they say.");
                        }
                        else if (funnyAppend == 3)
                        {
                            text = text.Insert(i + 1, " Mayhaps?");
                        }
                        else if (funnyAppend == 4)
                        {
                            text = text.Insert(i, " and shit");
                        }
                        else if (funnyAppend == 5)
                        {
                            text = text.Insert(i, " and stuff");
                        }
                    }
                }
            }

            return text;
        }

    }
}
