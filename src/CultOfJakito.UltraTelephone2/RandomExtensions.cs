namespace CultOfJakito.UltraTelephone2;

internal static class RandomExtensions {
	public static double NextDouble(this Random random, double minimumValue, double maximumValue) {
		return Remap(random.NextDouble(), 0, 1, minimumValue, maximumValue);
	}

	static double Remap(double s, double a1, double a2, double b1, double b2) {
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}
}
