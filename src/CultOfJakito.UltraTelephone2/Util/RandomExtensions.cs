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

    public static T LocateObjectButItActuallyFuckingWorks<T>(this Transform tf, string name) where T : Component
    {
        return tf.GetComponentsInChildren<T>().Where(x => x.name == name).FirstOrDefault();
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

    public static UnityEngine.AudioSource PlaySound(this UnityEngine.AudioClip clip, UnityEngine.Vector3? position = null, UnityEngine.Transform? parent = null, float volume = 1f, bool keepSource = false, bool loop = false)
    {
        UnityEngine.GameObject go = new UnityEngine.GameObject("Audio: " + clip.name);
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
