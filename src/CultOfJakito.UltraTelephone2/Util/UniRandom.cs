using System.Text;
using UnityEngine;

public class UniRandom : System.Random
{
    public static UniRandom Global
    {
        get
        {
            if (instance == null)
                throw new Exception("UniRandom global is not initialized. Please initialize it with InitializeGlobal");

            return instance;
        }
    }

    public static void InitializeGlobal()
    {
        if (instance != null)
            return;

        instance = CreateFullRandom();
        Debug.Log($"UniRandom initialized with global seed {instance.Seed}");
    }

    public static void InitializeGlobal(int seed)
    {
        if (instance != null)
            return;

        instance = new UniRandom(seed);
        Debug.Log($"UniRandom initialized with global seed {instance.Seed}");
    }

    public static void InitializeGlobal(string seed)
    {
        if (instance != null)
            return;

        instance = new UniRandom(seed);
        Debug.Log($"UniRandom initialized with global seed {instance.Seed}");
    }

    public static int GlobalSeed => instance.Seed;

    public static UniRandom GlobalRandom()
    {
        return new UniRandom(GlobalSeed);
    }

    public static UniRandom GlobalAdditive(int seed)
    {
        return new UniRandom(GlobalSeed^seed);
    }

    private static UniRandom globalSessionRandomizer;

    public static UniRandom SessionNext()
    {
        globalSessionRandomizer ??= new UniRandom(GlobalSeed);
        return new UniRandom(globalSessionRandomizer.Next());
    }

    public static void SessionGlobal(Action<UniRandom> rand)
    {
        rand.Invoke(GlobalRandom());
    }

    public static void DoRandom(Action<UniRandom> rand)
    {
        rand.Invoke(CreateFullRandom());
    }

    public static UniRandom CreateFullRandom()
    {
        return new UniRandom(Guid.NewGuid().GetHashCode());
    }
    public static void SetGloalSeed(int seed)
    {
        instance = new UniRandom(seed);
    }

    public static void SetGloalSeed(string seed)
    {
        instance = new UniRandom(seed);
    }

    private static UniRandom instance;
    public readonly int Seed;

    //GetHashCode is apparently not deterministic, so we need to convert the string to a numeric seed
    public static int StringToSeed(string seed)
    {
        if(string.IsNullOrEmpty(seed))
            return 0;

        int seedNumeric = 0;

        foreach (char c in seed)
        {
            seedNumeric ^= c;
        }

        return seedNumeric;
    }

    public UniRandom(int seed) : base(seed)
    {
        Seed = seed;
    }

    public UniRandom(string seed) : base(StringToSeed(seed))
    {
        Seed = StringToSeed(seed);
    }


    public UniRandom(UniRandom copySeedFrom) : base(copySeedFrom.Seed)
    {
        Seed = copySeedFrom.Seed;
    }

    public float Range(float min, float max)
    {
        return (float)this.NextDouble() * (max - min) + min;
    }

    public int Range(int minInclusive, int maxExclusive)
    {
        return this.Next(minInclusive, maxExclusive);
    }

    public T RandomEnum<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(this.Next(0, values.Length));
    }

    public float Float()
    {
        return (float)this.NextDouble();
    }

    public Quaternion Rotation()
    {
        return Quaternion.Euler(this.Range(0f, 360f), this.Range(0f, 360f), this.Range(0f, 360f));
    }

    public Vector3 InsideUnitSphere()
    {
        return UnitSphere() * this.Float();
    }

    public Vector3 UnitSphere()
    {
        return new Vector3(this.Range(-1f, 1f), this.Range(-1f, 1f), this.Range(-1f, 1f)).normalized;
    }

    public Vector3 PointInShell(float minRadius, float maxRadius)
    {
        return UnitSphere() * this.Range(minRadius, maxRadius);
    }

    public Vector3 PointOnShell(float radius)
    {
        return UnitSphere() * radius;
    }

    public Quaternion RandomRotationAxis(Vector3 axis)
    {
        return Quaternion.AngleAxis(this.Float() * 360f, axis);
    }

    public Vector3 PointInCone(Vector3 direction, float angle)
    {
        return Vector3.Slerp(direction, UnitSphere(), this.Float() * angle);
    }

    public Vector3 PointOnLine(Vector3 pointA, Vector3 pointB)
    {
        float t = this.Float();
        return pointA * (1f - t) + pointB * t;
    }

    public Vector3 UnitDisc(Vector3 normal)
    {
        Vector3 tangent = Vector3.Cross(normal, Vector3.up);
        if (tangent.sqrMagnitude < 0.01f)
            tangent = Vector3.Cross(normal, Vector3.right);
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(normal, tangent);
        bitangent.Normalize();
        float angle = this.Float() * Mathf.PI * 2f;
        float radius = Mathf.Sqrt(this.Float());
        return tangent * Mathf.Cos(angle) * radius + bitangent * Mathf.Sin(angle) * radius;
    }

    public Vector2 InsideUnitCircle()
    {
        return UnitCircle() * this.Float();
    }

    public Vector2 UnitCircle()
    {
        return new Vector2(this.Range(-1f, 1f), this.Range(-1f, 1f)).normalized;
    }

    public T SelectRandom<T>(ICollection<T> values)
    {
        if (values.Count == 0)
            return default(T);

        int index = this.Next(0, values.Count);
        int i = 0;

        foreach (T value in values)
        {
            if (i == index)
                return value;
            i++;
        }

        return default(T);
    }

    public T SelectRandomFromSet<T>(params T[] values)
    {
        if (values.Length == 0)
            return default(T);

        return values[this.Next(0, values.Length)];
    }

    //public T SelectRandom<T>(IEnumerable<T> values)
    //{
    //    return this.SelectRandomWeighted(values, _ => 1);
    //}

    public T SelectRandomWeighted<T>(IEnumerable<T> values, Func<T, int> weightSelector)
    {
        int totalWeight = 0;
        foreach (T value in values)
        {
            totalWeight += weightSelector(value);
        }

        int index = this.Next(0, totalWeight);
        foreach (T value in values)
        {
            int weight = weightSelector(value);
            if (index < weight)
                return value;
            index -= weight;
        }

        return default(T);

    }

    public bool Chance(float chance)
    {
        if (chance == 0)
            return false;

        if (chance == 1)
            return true;

        return this.NextDouble() < chance;
    }

    public bool Bool()
    {
        return this.Next(0, 2) == 1;
    }

    public string AlphabeticalString(int length)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append((char)this.Next(97, 123));
        }
        return sb.ToString();
    }

    public string AlphanumericString(int length)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            if (this.Bool())
                sb.Append((char)this.Next(97, 123));
            else
                sb.Append(this.Next(0, 10));
        }
        return sb.ToString();
    }

    public IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
    {
        List<T> list = new List<T>(source);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = this.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    public IEnumerable<T> TakeRandom<T>(IEnumerable<T> source, int count)
    {
        return this.Shuffle(source).Take(count);
    }

    public ulong ULong()
    {
        byte[] buffer = new byte[8];
        this.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }

    public long Long()
    {
        byte[] buffer = new byte[8];
        this.NextBytes(buffer);
        return BitConverter.ToInt64(buffer, 0);
    }

    public long Range(long minInclusive, long maxExclusive)
    {
        return (long)(this.ULong() % (ulong)(maxExclusive - minInclusive) + (ulong)minInclusive);
    }

    public ulong URange(ulong minInclusive, ulong maxExclusive)
    {
        return this.ULong() % (maxExclusive - minInclusive) + minInclusive;
    }

    public byte[] Bytes(int length)
    {
        byte[] buffer = new byte[length];
        this.NextBytes(buffer);
        return buffer;
    }

    public Color Color()
    {
        return new Color(this.Float(), this.Float(), this.Float(), this.Float());
    }

    public Color Color(float alpha)
    {
        return new Color(this.Float(), this.Float(), this.Float(), alpha);
    }

    public Color Color(float brightness, float saturation, float alpha = 1f)
    {
        Color color = UnityEngine.Color.HSVToRGB(this.Float(), saturation, brightness);
        color.a = alpha;
        return color;
    }

    public float Gaussian(float mean, float standardDeviation)
    {
        float u1 = 1.0f - this.Float();
        float u2 = 1.0f - this.Float();
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + standardDeviation * randStdNormal;
    }

    public Vector3 InBounds(Bounds bounds)
    {
        return new Vector3(this.Range(bounds.min.x, bounds.max.x), this.Range(bounds.min.y, bounds.max.y), this.Range(bounds.min.z, bounds.max.z));
    }

    public Vector3 InBox(Vector3 min, Vector3 max)
    {
        return new Vector3(this.Range(min.x, max.x), this.Range(min.y, max.y), this.Range(min.z, max.z));
    }

    public Vector2 InBox(Vector2 min, Vector2 max)
    {
        return new Vector2(this.Range(min.x, max.x), this.Range(min.y, max.y));
    }
}
