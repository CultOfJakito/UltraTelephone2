using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CultOfJakito.UltraTelephone2;

internal static class ServiceCollectionExtensions {
	public static void AddChaosEffect<T>(this IServiceCollection services) where T : UnityEngine.Component, IChaosEffect {
		services.AddScoped<IChaosEffect>(AddComponentAndInject<T>);
	}

	private static T AddComponentAndInject<T>(IServiceProvider services) where T : UnityEngine.Component {
		T comp = Plugin.BehaviourServiceHolder.AddComponent<T>();
		InjectionUtilities.InjectIntoExisting(comp, services);
		return comp;
	}
}
