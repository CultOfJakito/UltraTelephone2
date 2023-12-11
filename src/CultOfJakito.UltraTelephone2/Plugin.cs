using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace CultOfJakito.UltraTelephone2;

[BepInPlugin(nameof(CultOfJakito.UltraTelephone2), "Ultratelephone 2", "1.0.0")]
internal class Plugin : BaseUnityPlugin {
	private IServiceProvider _serviceProvider;
	private IServiceScope _currentScope;
	public static UnityEngine.GameObject BehaviourServiceHolder { get; private set; }

	private void Awake() {
		BehaviourServiceHolder = new UnityEngine.GameObject("UT2 Service Holder");
		DontDestroyOnLoad(BehaviourServiceHolder);

		new Harmony(Info.Metadata.GUID).PatchAll(Assembly.GetExecutingAssembly());

		ServiceCollection services = new();
		services.AddSingleton(this);
		services.AddSingleton(s => s.GetRequiredService<Plugin>().Logger);

		services.AddScoped(AddComponentAndInject<ChaosManager>);
		services.AddChaosEffect<OpenUrlOnDeath>();

		services.AddScoped<Random>();

		_serviceProvider = services.BuildServiceProvider();

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		_currentScope?.Dispose();
		_currentScope = null;
		//TODO: Only do this for levels
		Logger.LogInfo("Starting new service scope");
		_currentScope = _serviceProvider.CreateScope();
		_currentScope.ServiceProvider.GetRequiredService<ChaosManager>().BeginEffects();
	}

	private T AddComponentAndInject<T>(IServiceProvider services) where T : UnityEngine.Component {
		T comp = BehaviourServiceHolder.AddComponent<T>();
		InjectionUtilities.InjectIntoExisting(comp, services);
		return comp;
	}
}
