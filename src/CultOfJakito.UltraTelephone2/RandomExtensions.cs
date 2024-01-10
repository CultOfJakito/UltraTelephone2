using System.Runtime.CompilerServices;

namespace CultOfJakito.UltraTelephone2;

internal static class RandomExtensions
{
	public static double NextDouble(this Random random, double minimumValue, double maximumValue)
    {
		return Remap(random.NextDouble(), 0, 1, minimumValue, maximumValue);
	}

    public static void ForEach<T>(this IEnumerable<T> col, Action<T> action)
    {
        foreach(T item in col) action(item);
    }

	static double Remap(double s, double a1, double a2, double b1, double b2)
    {
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}

    public static bool Bool(this Random random)
    {
        return random.Next(2) == 0;
    }

    public static bool PercentChance(this Random random, float chance)
    {
        if (chance == 0f)
            return false;

        if (chance == 1f)
            return true;

        return random.NextDouble() < chance;
    }

	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
	{
		T[] elements = source.ToArray();
        for(int i = elements.Length - 1; i >= 0; i--)
		{
            int swapIndex = rng.Next(i + 1);
            yield return elements[swapIndex];
			elements[swapIndex] = elements[i];
        }
    }
	
	public static T RandomElement<T>(this IEnumerable<T> source, Random rng)
	{
		T[] elements = source.ToArray();
        return elements[rng.Next(elements.Length)];
    }
}
