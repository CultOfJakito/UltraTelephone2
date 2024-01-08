using System.Reflection;
using BepInEx;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Hydra;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2;

[BepInDependency("Hydraxous.ULTRAKILL.Configgy", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(nameof(CultOfJakito.UltraTelephone2), "Ultratelephone 2", "1.0.0")]
public class Plugin : BaseUnityPlugin {
	private IServiceProvider _serviceProvider;
	private IServiceScope _currentScope;
	public static UnityEngine.GameObject BehaviourServiceHolder { get; private set; }

    private AssetLoader _assetLoader;
    private ConfigBuilder _config;

    private void Awake() {
		BehaviourServiceHolder = new UnityEngine.GameObject("UT2 Service Holder");
		DontDestroyOnLoad(BehaviourServiceHolder);

        _config = new ConfigBuilder(nameof(CultOfJakito.UltraTelephone2), "Ultra Telephone 2");
        _config.Build();

        InGameCheck.Init();

		new Harmony(Info.Metadata.GUID).PatchAll(Assembly.GetExecutingAssembly());

		ServiceCollection services = new();
		services.AddSingleton(this);
		services.AddSingleton(s => s.GetRequiredService<Plugin>().Logger);

		//TODO update this to use the assetbundles.
		//_assetLoader = new AssetLoader(Resources.ut2assets);
		//services.AddSingleton(s => s.GetRequiredService<Plugin>()._assetLoader);

		services.AddScoped(AddComponentAndInject<ChaosManager>);
		services.AddChaosEffect<OpenUrlOnDeath>();
        services.AddChaosEffect<AnnoyingPopUp>();
		services.AddScoped<Random>();

		_serviceProvider = services.BuildServiceProvider();
        InGameCheck.OnLevelChanged += DoThing;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        if (SceneManager.GetActiveScene() != scene)
            return;

        _currentScope?.Dispose();
        _currentScope = null;

        if (!InGameCheck.InLevel())
            return;

		Logger.LogInfo("Starting new service scope");
		_currentScope = _serviceProvider.CreateScope();
		_currentScope.ServiceProvider.GetRequiredService<ChaosManager>().BeginEffects();

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

	private T AddComponentAndInject<T>(IServiceProvider services) where T : UnityEngine.Component {
		T comp = BehaviourServiceHolder.AddComponent<T>();
		InjectionUtilities.InjectIntoExisting(comp, services);
		return comp;
	}
}
