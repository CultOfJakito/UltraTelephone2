using CultOfJakito.UltraTelephone2.Assets;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2;

internal static class RandomExtensions
{
    public static void ForEach<T>(this IEnumerable<T> col, Action<T> action)
    {
        foreach (T item in col)
        {
            action(item);
        }
    }

    private static double Remap(double s, double a1, double a2, double b1, double b2) => b1 + (s - a1) * (b2 - b1) / (a2 - a1);

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, System.Random rng)
    {
        T[] elements = source.ToArray();
        for (int i = elements.Length - 1; i >= 0; i--)
        {
            int swapIndex = rng.Next(i + 1);
            yield return elements[swapIndex];
            elements[swapIndex] = elements[i];
        }
    }

    // what?
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

    public static AudioSource PlaySound(this AudioClip clip, Vector3? position = null, Transform? parent = null, float volume = 1f, bool keepSource = false, bool loop = false)
    {
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = position ?? Vector3.zero;

        if(parent != null)
            go.transform.parent = parent;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;

        if(position != null)
            source.spatialBlend = 1f;

        source.outputAudioMixerGroup = UkPrefabs.MainMixer;
        source.Play();

        if(!keepSource)
            UnityEngine.Object.Destroy(go, clip.length);

        return source;
    }
}
