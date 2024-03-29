using System.Reflection;
using BepInEx;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.Events;
using CultOfJakito.UltraTelephone2.Hydra;
using CultOfJakito.UltraTelephone2.Hydra.EA;
using CultOfJakito.UltraTelephone2.Util;
using CultOfJakito.UltraTelephone2.Zed;
using HarmonyLib;
using UltraTelephone.Hydra;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2;

[BepInDependency("Hydraxous.ULTRAKILL.Configgy")]
[BepInPlugin(MOD_GUID, MOD_NAME, VERSION)]
public class UltraTelephoneTwo : BaseUnityPlugin
{
    public const string VERSION = "1.0.0";
    public const string MOD_NAME = "UltraTelephone2";
    public const string MOD_GUID = "CultOfJakito.UltraTelephone2";
    public ChaosManager ChaosManager { get; private set; }
    public UniRandom Random { get; private set; }

    private ConfigBuilder _config;
    public static UltraTelephoneTwo Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        _config = new ConfigBuilder(nameof(UltraTelephone2), "Ultra Telephone 2");
        _config.Build();

        InGameCheck.Init();

        new Harmony(Info.Metadata.GUID).PatchAll(Assembly.GetExecutingAssembly());
        Patches.PatchAll();

        int globalSeed = PersonalizationLevelToSeed(GeneralSettings.Personalization.Value);
        Random = new UniRandom(globalSeed);
        UniRandom.InitializeGlobal(globalSeed);

        UT2Paths.EnsureFolders();
        AddressableManager.LoadCatalog();

        TextureHelper.LoadTextures(UT2Paths.TextureFolder);
        AudioHelper.LoadClips(UT2Paths.AudioFolder);

        InitializeObjects();

        InGameCheck.OnLevelChanged += DoThing;
        InGameCheck.OnLevelChanged += OnSceneLoaded;
    }

    private void InitializeObjects()
    {
        MinecraftBookPatch.Init();
        UT2TextFiles.ReloadFiles();
        HerobrineManager.Init();
        BuyablesManager.Load();
        GameEvents.OnPlayerHurt += (e) =>
        {
            if(e.Damage > 10)
            {
                Jumpscare.Scare();
            }
        };
    }


    private void OnSceneLoaded(string name)
    {
        if(GeneralSettings.Personalization.Value == PersonalizationLevel.ULTRAPERSONALIZED)
        {
            //full random seed.
            Random = UniRandom.CreateFullRandom();
        }

        if (!InGameCheck.InLevel())
        {
            return;
        }

        Logger.LogInfo("Starting new service scope");
        GameObject chaosManagerObject = new("UT2 Chaos Manager");
        ChaosManager = chaosManagerObject.AddComponent<ChaosManager>();
        ChaosManager.BeginEffects();
    }

    private void DoThing(string level)
    {
        if (!InGameCheck.InLevel())
        {
            return;
        }

        InGameCheck.OnLevelChanged -= DoThing;
        ModalDialogue.ShowSimple("ULTRATELEPHONE", "ULTRA TELEPHONE", _ => { }, "No?", "FUCK NO");
    }

    private static int PersonalizationLevelToSeed(PersonalizationLevel level)
    {
        switch (level)
        {
            default:
            case PersonalizationLevel.None:
                return 0;
            case PersonalizationLevel.NotMuch:
                return (int)DateTime.Now.DayOfWeek;
            case PersonalizationLevel.Some:
                return DateTime.Now.Day;
            case PersonalizationLevel.More:
                return (int)DateTime.Now.Hour;
            case PersonalizationLevel.Personalized:
                return Environment.UserName.GetHashCode();
            case PersonalizationLevel.ULTRAPERSONALIZED:
                return (int)DateTime.Now.Ticks^Environment.UserName.GetHashCode();
        }
    }
}

public enum PersonalizationLevel
{
    None,
    NotMuch,
    Some,
    More,
    Personalized,
    ULTRAPERSONALIZED,
}
