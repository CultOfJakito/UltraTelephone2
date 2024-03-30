namespace CultOfJakito.UltraTelephone2.Util
{
    public class SeedBuilder
    {
        private List<int> seedModifiers;

        public SeedBuilder()
        {
            seedModifiers = new List<int>();
        }

        public SeedBuilder Reverse()
        {
            seedModifiers.Reverse();
            return this;
        }

        public SeedBuilder WithSeed(int seedModifier)
        {
            seedModifiers.Add(seedModifier);
            return this;
        }

        public SeedBuilder WithSeed(string seedModifier)
        {
            seedModifiers.Add(UniRandom.StringToSeed(seedModifier));
            return this;
        }

        public SeedBuilder WithObjectHash(object seedModifier)
        {
            seedModifiers.Add(seedModifier.GetHashCode());
            return this;
        }

        public SeedBuilder WithSceneName()
        {
            seedModifiers.Add(UniRandom.StringToSeed(SceneHelper.CurrentScene));
            return this;
        }

        public SeedBuilder WithGlobalSeed()
        {
            seedModifiers.Add(UltraTelephoneTwo.Instance.Random.Seed);
            return this;
        }

        public SeedBuilder WithFullRandom()
        {
            seedModifiers.Add(UniRandom.CreateFullRandom().Next());
            return this;
        }

        public int GetSeed()
        {
            int seed = seedModifiers.Count > 0 ? seedModifiers[0] : 0;
            for (int i = 1; i < seedModifiers.Count; i++)
            {
                seed ^= seedModifiers[i];
            }
            return seed;
        }

        public override int GetHashCode()
        {
            return GetSeed();
        }

        public static implicit operator int(SeedBuilder seedBuilder)
        {
            return seedBuilder.GetSeed();
        }
    }
}
