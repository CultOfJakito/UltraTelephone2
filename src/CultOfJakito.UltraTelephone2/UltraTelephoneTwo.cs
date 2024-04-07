using System.Reflection;
using BepInEx;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.Chaos.Effects;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.Events;
using CultOfJakito.UltraTelephone2.Fun;
using CultOfJakito.UltraTelephone2.Fun.Coin;
using CultOfJakito.UltraTelephone2.Fun.EA;
using CultOfJakito.UltraTelephone2.Fun.Glungus;
using CultOfJakito.UltraTelephone2.Fun.Herobrine;
using CultOfJakito.UltraTelephone2.LevelInjection;
using CultOfJakito.UltraTelephone2.Patches;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2;

[BepInDependency("Hydraxous.ULTRAKILL.Configgy")]
[BepInPlugin(MOD_GUID, MOD_NAME, VERSION)]
[HarmonyPatch]
public class UltraTelephoneTwo : BaseUnityPlugin
{
    public const string VERSION = "1.1.0";
    public const string MOD_NAME = "UltraTelephone2";
    public const string MOD_GUID = "CultOfJakito.UltraTelephone2";
    public ChaosManager ChaosManager { get; private set; }
    public UniRandom Random { get; private set; }

    private ConfigBuilder _config;
    public static UltraTelephoneTwo Instance { get; private set; }

    const int MAX_LOG_LINES = 100;
    public static List<string> LogBuffer;

    private void Awake()
    {
        Instance = this;
        AddressableManager.LoadCatalog();

        LogBuffer = new List<string>();
        Application.logMessageReceived += Application_logMessageReceived;

        _config = new ConfigBuilder(nameof(UltraTelephone2), "Ultra Telephone 2");
        _config.Build();

        InGameCheck.Init();

        new Harmony(Info.Metadata.GUID).PatchAll(Assembly.GetExecutingAssembly());

        int globalSeed = PersonalizationLevelToSeed(GeneralSettings.Personalization.Value);

        if(DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
        {
            Debug.LogWarning("Happy April Fools! UT2 Seed is 69^YOU for the day!!");
            globalSeed = 69^UniRandom.StringToSeed(Environment.UserName);
        }

        Random = new UniRandom(globalSeed);
        UniRandom.InitializeGlobal(globalSeed);

        BackupSaveData.EnsureBackupExists();

        //UT2Assets.ValidateAssetIntegrity();
        UT2Paths.EnsureFolders();
        UT2SaveData.Load();

        Jumpscare.ValidateFiles();
        TextureHelper.LoadTextures(UT2Paths.TextureFolder);
        TextDestruction.Initialize();
        //AudioHelper.LoadClips(UT2Paths.AudioFolder); Unused for now.

        InitializeObjects();

        InGameCheck.OnLevelChanged += OnSceneLoaded;
        AutoSaveUpdate();
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        LogBuffer.Insert(0, condition);
        if (LogBuffer.Count > MAX_LOG_LINES)
            LogBuffer.RemoveAt(LogBuffer.Count - 1);
    }

    private void InitializeObjects()
    {
        LoadBearingCoconut.EnsureStability();

        gameObject.AddComponent<LevelInjectionManager>();

        AlterFriendAvatars.Load();
        MinecraftBookPatch.Init();
        UT2TextFiles.ReloadFiles();
        HerobrineManager.Init(); //Herobrine is busted af right now bc of script serialization issues
        BuyablesManager.Load();

        GameEvents.OnEnemyDeath += CoinCollectable.OnEnemyDeath;
        GameEvents.OnEnemyDeath += (v) =>
        {
            if(Random.Chance(0.05f))
            {
                AnnoyingPopUp.OkDialogue("Nice Job!", "Good job killing that enemy!");
            }
        };


        GameEvents.OnLevelStateChange += _ => RandomWindowTitle.Reroll();
        RandomWindowTitle.Reroll();
    }


    private void OnSceneLoaded(string name)
    {
        if(GeneralSettings.Personalization.Value == PersonalizationLevel.ULTRAPERSONALIZED)
        {
            //full random seed.
            Random = UniRandom.CreateFullRandom();
        }

        CantRead.IgnoreBookGameObjectHashes.Clear();

        if (!InGameCheck.InLevel())
        {
            return;
        }

        Logger.LogInfo("Starting new service scope");
        GameObject chaosManagerObject = new("UT2 Chaos Manager");
        ChaosManager = chaosManagerObject.AddComponent<ChaosManager>();
        ChaosManager.BeginEffects();

        GameObject glungusManager = new("UT2 Glungus Manager");
        glungusManager.AddComponent<GlungusManager>();
    }


    private void Update()
    {

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7))
        {
            AddressableManager.LoadSceneUnsanitzed(EnterCasinoPatch.CASINO_SCENE_NAME);
        }
    }

    private void AutoSaveUpdate()
    {
        if(UT2SaveData.IsDirty)
            UT2SaveData.Save();

        Invoke(nameof(AutoSaveUpdate), 5f);
    }

    private void OnApplicationQuit()
    {
        //Make sure to save on quit so we don't lose data!
        UT2SaveData.Save();
    }


    private static int PersonalizationLevelToSeed(PersonalizationLevel level)
    {
        switch (level)
        {
            default:
            case PersonalizationLevel.None:
                return 0;
            case PersonalizationLevel.Personalized:
                return UniRandom.StringToSeed(Environment.UserName);
            case PersonalizationLevel.NotMuch:
                return (int)DateTime.Now.DayOfWeek;
            case PersonalizationLevel.Some:
                return DateTime.Now.Day;
            case PersonalizationLevel.More:
                return DateTime.Now.Hour^(DateTime.Now.Minute%20);
            case PersonalizationLevel.ULTRAPERSONALIZED:
                return (int)DateTime.Now.Ticks^UniRandom.StringToSeed(Environment.UserName);
        }
    }

    [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitCyberGrindScore))]
    [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitLevelScore))]
    [HarmonyPrefix]
    public static bool DisableCg()
    {
        return false;
    }
}

public enum PersonalizationLevel
{
    None,
    Personalized,
    NotMuch,
    Some,
    More,
    ULTRAPERSONALIZED,
}
