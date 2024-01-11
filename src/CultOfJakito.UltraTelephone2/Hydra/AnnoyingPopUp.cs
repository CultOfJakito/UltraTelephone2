using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CultOfJakito.UltraTelephone2.Chaos
{
    [RegisterChaosEffect]
    public class AnnoyingPopUp : ChaosEffect
    {
        [Configgable("Hydra", "Show Annoying Death Messages")]
        private static ConfigToggle showAnnoyingPopUps = new ConfigToggle(true);

        private System.Random rng;

        public override void BeginEffect(System.Random random)
        {
            rng = random;
            randomDialogueEvent = new ModalDialogueEvent();
            GeneratePopups();
            EventBus.RestartedFromCheckpoint += OnRestartedFromCheckPoint;
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            bool canBegin = base.CanBeginEffect(ctx);

            if (!canBegin)
                return false;

            return showAnnoyingPopUps.Value;
        }

        public override int GetEffectCost()
        {
            return 1;
        }

        private void OnDestroy()
        {
            EventBus.RestartedFromCheckpoint -= OnRestartedFromCheckPoint;
        }

        private void OnRestartedFromCheckPoint(bool playerDied)
        {
            if (playerDied)
                ShowPopUp();
        }

        private void ShowPopUp()
        {
            ModalDialogueEvent dialogue = null;

            if (rng.PercentChance(0.25f))
                dialogue = dialogueBuilders.RandomElement(rng).Invoke();
            else
                dialogue = CreateRandomized();

            ModalDialogue.ShowDialogue(dialogue);
        }

        private DialogueBoxOption[] evilOptions;

        private void GeneratePopups()
        {
            evilOptions = new DialogueBoxOption[]
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
                    UKPrefabs.MinosPrime.LoadObjectAsync((status, result) =>
                    {
                        if (status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        {
                            GameObject.Instantiate(result, pos, Quaternion.identity);
                        }
                    });
                }
            },
            
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Skip Level",
                OnClick = SkipLevel
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Spawn Some MindFlayers",
                OnClick = () =>
                {
                    Vector3 pos = NewMovement.Instance.transform.position;
                    UKPrefabs.MindFlayer.LoadObjectAsync((status, result) =>
                    {
                        if(status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        {
                            int count = UltraTelephoneTwo.Instance.Random.Next(3,17);
                            for(int i = 0; i < count; i++)
                            {
                                GameObject mfGo = GameObject.Instantiate(result, pos, Quaternion.identity);
                                EnemyIdentifier mf = mfGo.GetComponent<EnemyIdentifier>();
                                mf.radianceTier = 3;
                                mf.speedBuff = true;
                                mf.damageBuff = true;
                                mf.healthBuff = true;
                                mf.UpdateBuffs();
                            }
                        }
                    });
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
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Restart Mission",
                OnClick = () =>
                {
                    OptionsManager.Instance.RestartMission();
                }
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Explode",
                OnClick = () =>
                {
                    Vector3 pos = NewMovement.Instance.transform.position;
                    UKPrefabs.MinosPrimeExplosion.LoadObjectAsync((s,r) =>
                    {
                        if(s == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        {
                            GameObject.Instantiate(r, pos, Quaternion.identity);
                        }
                    });
                }
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = "Delete All Save-Data",
                OnClick = DeleteAllSaveDataFake
            },
            new DialogueBoxOption()
            {
                Color = Color.red,
                Name = $"Spend ({FakeBank.PString(1000)})",
                OnClick = () =>
                {
                    FakeBank.AddMoney(-1000);
                    OkDialogue("Purchase Confirmed.", $"Your purchase has been completed. \nYour balance is now\n({FakeBank.PString(FakeBank.GetCurrentMoney())})");
                }
            }
        };
            dialogueBuilders = new Func<ModalDialogueEvent>[]
            {
                CreateDeathFee,
                CreateTutorialPopUp,
                CreateEndOfDemo,
            };

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

        private ModalDialogueEvent randomDialogueEvent;
        private Func<ModalDialogueEvent>[] dialogueBuilders;
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
                    if (rng.NextDouble() > 0.75f)
                    {
                        if (rng.Bool())
                            ShowPopUp();
                    }
                }
            };
        }

       

        private void SkipLevel()
        {
            bool inLevel = InGameCheck.InLevel();
            if (!inLevel)
            {
                ModalDialogue.ShowSimple("You're not in a level!", "You're not in a level!", (b) =>
                {
                    if (!b)
                        OkDialogue("You better be.", "Yeah you should be.");

                }, "Ok", "Im sorry...");
                return;
            }

            string levelName = SceneHelper.CurrentScene;
            int hyphenIndex = levelName.IndexOf('-');

            if (hyphenIndex == -1)
                return;

            if (!int.TryParse(levelName[hyphenIndex - 1].ToString(), out int layerIndex))
                return;

            if (!int.TryParse(levelName[hyphenIndex + 1].ToString(), out int levelIndex))
                return;

            int nextLayer = 0;
            int nextLevel = 0;

            if (layerIndex == 0)
            {
                if (levelIndex == 5)
                {
                    nextLayer = 1;
                    nextLevel = 1;
                }
                else
                {
                    nextLevel = levelIndex + 1;
                }
            }
            else
            {
                nextLayer = layerIndex;
                if ((layerIndex % 3 == 0 && levelIndex == 2) || levelIndex == 4)
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

        private void DeleteAllSaveDataFake()
        {
            ModalDialogue.ShowSimple("Are you sure?", "Are you sure you want to delete all save data?", (result) =>
            {
                ModalDialogue.ShowDialogue(new ModalDialogueEvent()
                {
                    Message = "Deleting Save Data.",
                    Title = "Deleting Save Data.",
                    Options = new DialogueBoxOption[]
                    {
                                new DialogueBoxOption()
                                {
                                    Color = Color.red,
                                    Name = "Ok",
                                    OnClick = () =>
                                    {
                                        if (UnityEngine.Random.value > 0.5f)
                                        {
                                            Application.Quit();
                                        }
                                        else
                                        {
                                            ModalDialogue.ShowSimple("Error", "Save data is already deleted.", (_)=>{ }, "OK", "OK");
                                        }
                                    }
                                }
                    }
                });
            }, "Yeah, no.", "No, I'm positive.");
        }

        private int tutorialPopUpCount = 0;
        private ModalDialogueEvent CreateTutorialPopUp()
        {
            ModalDialogueEvent modalDialogue = new ModalDialogueEvent();
            modalDialogue.Title = "Tutorial";
            switch (tutorialPopUpCount)
            {
                case 0:
                    modalDialogue.Message = "Welcome to Ultrakill! If you are having trouble you can play the in-game tutorial!";
                    modalDialogue.Options = new DialogueBoxOption[]
                    {
                        new DialogueBoxOption()
                        {
                            Color = orange,
                            Name = "Play Tutorial",
                            OnClick = () =>
                            {
                                SceneHelper.LoadScene("Tutorial");
                            }
                        },new DialogueBoxOption()
                        {
                            Color = Color.red,
                            Name = "No thank you.",
                            OnClick = () => { }
                        },
                    };
                    break;
                case 1:
                    modalDialogue.Message = "You look like you're having a little trouble. The tutorial may help you.";
                    modalDialogue.Options = new DialogueBoxOption[]
                    {
                        new DialogueBoxOption()
                        {
                            Color = orange,
                            Name = "Play Tutorial",
                            OnClick = () =>
                            {
                                SceneHelper.LoadScene("Tutorial");
                            }
                        },new DialogueBoxOption()
                        {
                            Color = Color.red,
                            Name = "No thank you.",
                            OnClick = () =>
                            {
                                ModalDialogue.ShowDialogue(new ModalDialogueEvent()
                                {
                                    Message = "Are you sure?",
                                    Title = "Are you sure?",
                                    Options = new DialogueBoxOption[]
                                    {
                                        new DialogueBoxOption()
                                        {
                                            Color = Color.green,
                                            Name = "Take me to the Tutorial",
                                            OnClick = () =>
                                            {
                                                SceneHelper.LoadScene("Tutorial");
                                            }
                                        },
                                        new DialogueBoxOption()
                                        {
                                            Color = Color.red,
                                            Name = "I'll think about it.",
                                            OnClick = () => { }
                                        }
                                    }
                                });
                            }
                        },
                    };
                    break;
                default:
                    modalDialogue.Message = "I'm not asking anymore. You will play the tutorial. Watching you play is like drinking a glass of orange juice after eating an entire pack of mentos.";
                    modalDialogue.Options = new DialogueBoxOption[]
                    {
                        new DialogueBoxOption()
                        {
                            Color = Color.red,
                            Name = "Play Tutorial",
                            OnClick = () =>
                            {
                                SceneHelper.LoadScene("Tutorial");
                            }
                        }
                    };
                    break;
            }

            //hehe switch the order sometimes
            if (rng.PercentChance(0.25f) && tutorialPopUpCount == 0 || tutorialPopUpCount == 1)
            {
                DialogueBoxOption op1 = modalDialogue.Options[0];
                modalDialogue.Options[0] = modalDialogue.Options[1];
                modalDialogue.Options[1] = op1;
            }

            tutorialPopUpCount++;

            return modalDialogue;
        }

        private ModalDialogueEvent CreateDeathFee()
        {
            long money = FakeBank.GetCurrentMoney();
            int divisor = rng.Next(50, 101);
            long fee = money / divisor;

            return new ModalDialogueEvent()
            {
                Message = $"You died. You must pay a fee to continue.",
                Title = "You died.",
                Options = new DialogueBoxOption[]
                {
                    new DialogueBoxOption()
                    {
                        Color = Color.red,
                        Name = $"Continue ({FakeBank.PString(fee)})",
                        OnClick = () =>
                        {
                            FakeBank.AddMoney(-fee);
                            OkDialogue("Purchase Confirmed.", $"Your purchase has been completed. \nYour balance is now\n({FakeBank.PString(FakeBank.GetCurrentMoney())})");
                        }
                    }
                }
            };
        }

        private ModalDialogueEvent CreateEndOfDemo()
        {
            long gameCost = 250000;

            return new ModalDialogueEvent()
            {
                Message = $"Thanks for trying out Ultrakill! You have reached the end of the demo! Please purchase the full game to continue!",
                Title = "Thanks for playing!",
                Options = new DialogueBoxOption[]
                {
                    new DialogueBoxOption()
                    {
                        Color = Color.red,
                        Name = $"Continue ({FakeBank.PString(gameCost)})",
                        OnClick = () =>
                        {
                            if(FakeBank.GetCurrentMoney() < gameCost)
                            {
                                OkDialogue("Purchase Failed.", $"You don't have that much money.");
                                return;
                            }

                            FakeBank.AddMoney(-gameCost);
                            OkDialogue("Purchase Confirmed.", $"Your purchase has been completed. \nYour balance is now\n({FakeBank.PString(FakeBank.GetCurrentMoney())})\nEnjoy the game!");
                        }
                    },
                    new DialogueBoxOption()
                    {
                        Color = Color.red,
                        Name = "QUIT",
                        OnClick = () =>
                        {
                            Application.Quit();
                        }
                    }
                }
            };
        }

        private ModalDialogueEvent CreateRandomized()
        {
            int options = rng.Next(1, 4);
            randomDialogueEvent.Message = messages.RandomElement(rng);
            randomDialogueEvent.Title = titles.RandomElement(rng);
            randomDialogueEvent.Options = new DialogueBoxOption[options];
            for(int i = 0; i < randomDialogueEvent.Options.Length; i++)
            {
                randomDialogueEvent.Options[i] = CreateRandomOption();
            }
            return randomDialogueEvent;
        }

        public static void OkDialogue(string title, string message, string option = "Ok", Action onClick = null)
        {
            ModalDialogue.ShowDialogue(new ModalDialogueEvent()
            {
                Message = message,
                Title = title,
                Options = new DialogueBoxOption[]
                {
                    new DialogueBoxOption()
                    {
                        Color = orange,
                        Name = option,
                        OnClick = onClick
                    }
                }
            });
        }

    }
}
