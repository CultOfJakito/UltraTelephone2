using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CultOfJakito.UltraTelephone2.DependencyInjection;

internal static class InjectionUtilities {
	public static void InjectIntoExisting(object obj, IServiceProvider serviceProvider) {
		foreach(PropertyInfo property in obj.GetType()
			.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(x => x.GetCustomAttribute<InjectAttribute>() != null)) {
			property.SetValue(obj, serviceProvider.GetRequiredService(property.PropertyType));
		}
	}
}
