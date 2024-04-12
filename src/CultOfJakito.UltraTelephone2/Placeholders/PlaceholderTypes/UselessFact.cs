using Newtonsoft.Json;

namespace CultOfJakito.UltraTelephone2.Placeholders.PlaceholderTypes
{
    public class UselessFact : IStringPlaceholder
    {
        static List<string> cachedFacts;

        const string DEFAULT_FACT = "Human beings are silly creatures... :3";
        private static UniRandom rng;

        public static string GetUselessFact()
        {
            cachedFacts ??= new List<string>();
            rng ??= UniRandom.CreateFullRandom();
            RequestFact();

            if(cachedFacts.Count <= 0)
                return DEFAULT_FACT;

            return rng.SelectRandom(cachedFacts);
        }

        private async static Task<string> GetFact()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("https://uselessfacts.jsph.pl/random.json?language=en");

                if (!response.IsSuccessStatusCode)
                    return null;

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic fact = JsonConvert.DeserializeObject(json);
                    return fact.text;
                }
            }

            return null;
        }

        private static void RequestFact()
        {
            Task.Run(async () =>
            {
                string fact = await GetFact();

                if(!string.IsNullOrEmpty(fact))
                    cachedFacts.Add(fact);
            });
        }

        public string PlaceholderID => "USELESS_FACT";

        public string GetPlaceholderValue()
        {
            return GetUselessFact();
        }

        public UselessFact()
        {
            RequestFact();
        }
    }
}
