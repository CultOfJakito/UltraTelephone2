using System.Reflection;
using BepInEx;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.Zed;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2;

[BepInDependency("Hydraxous.ULTRAKILL.Configgy")]
[BepInPlugin(nameof(UltraTelephone2), "UltraTelephone 2", "1.0.0")]
public class UltraTelephoneTwo : BaseUnityPlugin
{
    public AssetLoader AssetLoader { get; private set; }

    public AssetLoader ZelzmiyBundle;

    public ChaosManager ChaosManager { get; private set; }
    public System.Random Random { get; private set; }

    private ConfigBuilder _config;
    public static UltraTelephoneTwo Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        _config = new ConfigBuilder(nameof(UltraTelephone2), "Ultra Telephone 2");
        _config.Build();

        Data.Paths.ValidateFolders();
        InGameCheck.Init();

        new Harmony(Info.Metadata.GUID).PatchAll(Assembly.GetExecutingAssembly());

        string username = Environment.UserName;
        int dayOfTheWeek = (int)DateTime.Now.DayOfWeek;

        Random = new System.Random(username.GetHashCode() + dayOfTheWeek);

        ZelzmiyBundle = new AssetLoader(Data.Paths.GetBundleFilePath("Zelzmiy.resource"));

        InGameCheck.OnLevelChanged += DoThing;
        MinecraftBookPatch.Init();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene() != scene)
        {
            return;
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
}
