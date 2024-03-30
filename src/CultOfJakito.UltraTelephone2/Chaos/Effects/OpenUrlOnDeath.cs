using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

[RegisterChaosEffect]
public class OpenUrlOnDeath : ChaosEffect
{
    [Configgable("Chaos/Effects", "Open URL On Death")]
    private static ConfigToggle s_openUrlOnDeath = new(true);

    private static readonly string[] s_urlPool =
    [
        CreateGoogleSearchUrl("why am i so bad at video games"),
        CreateGoogleSearchUrl("ultrakill how to enable easy mode"),
        CreateGoogleSearchUrl("ultrakill cheese strats"),
        CreateGoogleSearchUrl("ultrakill cheat engine table"),
        CreateGoogleSearchUrl("ultrakill trainer free download"),
        CreateGoogleSearchUrl("ultrakill epic swag guide"),
        CreateGoogleSearchUrl("how to get better at ultrakill"),
        CreateGoogleSearchUrl("suicide hotline"),
        CreateGoogleSearchUrl("suicide hotline free download"),
        CreateGoogleSearchUrl("hot robots in my area"),
        CreateGoogleSearchUrl("minecraft speedbridge tutorial"),
        CreateGoogleSearchUrl("brutwurst recepie"),
        CreateGoogleSearchUrl("how to write a good job resume and cover letter"),
        CreateGoogleSearchUrl("\"not for public release\" filetype:pdf site:s3.amazonaws.com"), // leaked CIA documents
        //"https://twitter.com/prezoh/status/1569535221707448320", // debating on weather to include this one or not but it's pretty funny
        "https://en.wikipedia.org/wiki/Special:Random", // random wikipedia page
        "https://youtube.com/playlist?list=PLtr1CuIZfdMAwqqRa29SrZhuwzPyKOGqw", // herbmessiah ultrakill guides
        "https://www.youtube.com/watch?v=nMJExYjDjtI", // you're alone in the lab at night because the product is still impure (a chemistry playlist)
        "https://www.youtube.com/watch?v=SZdf_6cBE-o&list=PLKBRHzyVsSQMJ76Pwc1quZTcHyhqegGTs", // calculus tutorial playlist
        "https://outrightinternational.org/take-action/give", // charity lol 
        "https://store.steampowered.com/app/1890950/REAVER/",
        "https://store.steampowered.com/app/1313140/Cult_of_the_Lamb/"
    ];


    private List<string> _urlPool;

    private float _chance;
    private UniRandom _random;

    public override void BeginEffect(UniRandom random)
    {
        _chance = random.Range(0.5f, 0.85f);
        _random = random;
        _urlPool = new List<string>(s_urlPool);

        //Register death event
        GameEvents.OnPlayerDeath += OnPlayerDied;
        Debug.Log("Chance to open URL is " + _chance);
    }


    private void OnPlayerDied()
    {
        if (_random.Float() <= _chance)
        {
            if (_urlPool.Count <= 0)
            {
                _urlPool.AddRange(s_urlPool);
            }

            int index = _random.Next(_urlPool.Count);
            string url = _urlPool[index];
            Debug.Log($"Opening URL {url}");
            _urlPool.RemoveAt(index);
            Application.OpenURL(url);
        }
    }

    public override bool CanBeginEffect(ChaosSessionContext ctx) => base.CanBeginEffect(ctx) && s_openUrlOnDeath.Value;
    public override int GetEffectCost() => 1;

    private void OnDestroy()
    {
        GameEvents.OnPlayerDeath -= OnPlayerDied;
    }

    private static string CreateGoogleSearchUrl(string query) => $"https://google.com/search?q={query.Replace(' ', '+')}";
}
