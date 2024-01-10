using System.Reflection;
using BepInEx;
using Configgy;
using Configgy.UI;
using CultOfJakito.UltraTelephone2.Chaos;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2;

[BepInDependency("Hydraxous.ULTRAKILL.Configgy", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(nameof(CultOfJakito.UltraTelephone2), "Ultratelephone 2", "1.0.0")]
public class UltraTelephoneTwo : BaseUnityPlugin
{

    public AssetLoader AssetLoader { get; private set; }
    public ChaosManager ChaosManager { get; private set; }
    public System.Random Random { get; private set; }

    private ConfigBuilder _config;

    public static UltraTelephoneTwo Instance { get; private set; }

    private void Awake()
    {

        Instance = this;

        _config = new ConfigBuilder(nameof(CultOfJakito.UltraTelephone2), "Ultra Telephone 2");
        _config.Build();

        InGameCheck.Init();

		new Harmony(Info.Metadata.GUID).PatchAll(Assembly.GetExecutingAssembly());
        Harmony.CreateAndPatchAll(typeof(BouncyCannonballPatch));

        string username = Environment.UserName;
        int dayOfTheWeek = (int)DateTime.Now.DayOfWeek;

        Random = new System.Random(username.GetHashCode()+dayOfTheWeek);

        //TODO update this to use the assetbundles.
        //_assetLoader = new AssetLoader(Resources.ut2assets);

        InGameCheck.OnLevelChanged += DoThing;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (SceneManager.GetActiveScene() != scene)
            return;

        if (!InGameCheck.InLevel())
            return;

		Logger.LogInfo("Starting new service scope");
        GameObject chaosManagerObject = new GameObject("UT2 Chaos Manager");
        ChaosManager = chaosManagerObject.AddComponent<ChaosManager>();

        ChaosManager.BeginEffects();
    }

    private void DoThing(string level)
    {
        if (!InGameCheck.InLevel())
            return;

        InGameCheck.OnLevelChanged -= DoThing;
        ModalDialogue.ShowSimple("ULTRATELEPHONE", "ULTRA TELEPHONE", (b) =>
        {

        }, "No?", "FUCK NO");

    }
}
