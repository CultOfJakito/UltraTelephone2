namespace CultOfJakito.UltraTelephone2;

internal static class RandomExtensions
{
    public static double NextDouble(this Random random, double minimumValue, double maximumValue) => Remap(random.NextDouble(), 0, 1, minimumValue, maximumValue);

    public static void ForEach<T>(this IEnumerable<T> col, Action<T> action)
    {
        foreach (T item in col)
        {
            action(item);
        }
    }

    private static double Remap(double s, double a1, double a2, double b1, double b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);

    public static bool Bool(this Random random) => random.Next(2) == 0;

    public static bool PercentChance(this Random random, float chance)
    {
        switch (chance)
        {
            case 0f:
                return false;
            case 1f:
                return true;
            default:
                return random.NextDouble() < chance;
        }
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
    {
        T[] elements = source.ToArray();
        for (int i = elements.Length - 1; i >= 0; i--)
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

    public static bool FastStartsWith(this string str, string value)
    {
        if (value.Length > str.Length)
        {
            return false;
        }
        for (int i = 0; i < value.Length; i++)
        {
            if (str[i] != value[i])
            {
                return false;
            }
        }
        return true;
    }
    public static UnityEngine.AudioSource PlaySound(this UnityEngine.AudioClip clip, UnityEngine.Vector3? position = null, UnityEngine.Transform? parent = null, float volume = 1f, bool keepSource = false, bool loop = false)
    {
        UnityEngine.GameObject go = new UnityEngine.GameObject("Audio: " + clip.name);
        go.AddComponent<Marker>().Name = "PlaySound";
        go.transform.position = position ?? UnityEngine.Vector3.zero;
        if(parent != null) go.transform.parent = parent;
        UnityEngine.AudioSource source = go.AddComponent<UnityEngine.AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.Play();
        if(!keepSource) UnityEngine.Object.Destroy(go, clip.length);
        return source;
    }
}
