using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CultOfJakito.UltraTelephone2.Chaos
{
    [RegisterChaosEffect]
    public class AnnoyingPopUp : ChaosEffect
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
            Debug.Log("POP UP REG");
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


        private DialogueBoxOption[] evilOptions = new DialogueBoxOption[]
        {
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "QUIT",
                OnClick = () => { Application.Quit(); }
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Spawn Minos Prime",
                OnClick = () =>
                {
                    Vector3 pos = NewMovement.Instance.transform.position;
                    Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Enemies/MinosPrime.prefab").Completed += (g) =>
                    {
                        if(g.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        {
                            GameObject.Instantiate(g.Result, pos, Quaternion.identity);
                        }
                    };
                }
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Play Tutorial",
                OnClick = () =>
                {
                    SceneHelper.LoadScene("Tutorial");
                }
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Skip Level",
                OnClick = () =>
                {
                    bool inLevel = InGameCheck.InLevel();
                    if(!inLevel)
                    {
                        ModalDialogue.ShowSimple("You're not in a level!", "You're not in a level!", (b) => { }, "Ok", "Im sorry...");
                        return;
                    }

                    string levelName = SceneHelper.CurrentScene;
                    int hyphenIndex = levelName.IndexOf('-');

                    if(hyphenIndex == -1)
                        return;

                    if(int.TryParse(levelName[hyphenIndex-1].ToString(), out int layerIndex))
                    {
                        if(int.TryParse(levelName[hyphenIndex+1].ToString(), out int levelIndex))
                        {
                            int nextLayer = 0;
                            int nextLevel = 0;

                            if(layerIndex == 0)
                            {
                                if(levelIndex == 5)
                                {
                                    levelIndex = 1;
                                    levelIndex = 1;
                                }
                                else
                                {
                                    nextLevel = levelIndex + 1;
                                    nextLayer = 0;
                                }
                            }
                            else if(layerIndex % 3 == 0 && levelIndex == 2)
                            {
                                nextLayer = layerIndex;

                                if((layerIndex % 3 == 0 && levelIndex == 2) || levelIndex == 4)
                                {
                                    nextLayer = layerIndex + 1;
                                    nextLevel = 1;
                                }
                                else
                                {
                                    nextLevel = levelIndex + 1;
                                }
                            }

                            SceneHelper.LoadScene($"Level {nextLayer}-{nextLevel}");
                        }
                    }
                }
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Spawn Some MindFlayers",
                OnClick = () =>
                {
                    Vector3 pos = NewMovement.Instance.transform.position;
                    Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Enemies/Mindflayer.prefab").Completed += (g) =>
                    {
                        int count = UltraTelephoneTwo.Instance.Random.Next(3,15);
                        if(g.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        {
                            for(int i = 0; i < count; i++)
                            {
                                GameObject mfGo = GameObject.Instantiate(g.Result, pos, Quaternion.identity);
                                EnemyIdentifier mf = mfGo.GetComponent<EnemyIdentifier>();
                                mf.radianceTier = 3;
                                mf.speedBuff = true;
                                mf.damageBuff = true;
                                mf.healthBuff = true;
                                mf.UpdateBuffs();
                            }
                        }
                    };
                }
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Enable Radiance Tier 10",
                OnClick = () =>
                {
                    OptionsManager.forceRadiance = true;
                    OptionsManager.radianceTier = 10;
                    foreach (var eid in GameObject.FindObjectsOfType<EnemyIdentifier>())
                    {
                        eid.UpdateBuffs(false);
                    }
                }
            }
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
            if (rng.PercentChance(0.1f))
            {
                return evilOptions.RandomElement(rng);
            }

            return new DialogueBoxOption()
            {
                Name = optionNames.RandomElement(rng),
                Color = orange,
                OnClick = () =>
                {
                    if(rng.NextDouble() > 0.75f)
                    {
                        if(rng.Bool())
                            ShowPopUp();
                    }
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
