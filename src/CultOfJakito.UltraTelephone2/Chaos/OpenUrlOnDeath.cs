using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Chaos;

[RegisterChaosEffect]
public class OpenUrlOnDeath : ChaosEffect
{
    [Configgable("TeamDoodz", "Open URL On Death")]
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
        "https://youtube.com/playlist?list=PLtr1CuIZfdMAwqqRa29SrZhuwzPyKOGqw", // herbmessiah ultrakill guides
        "https://store.steampowered.com/app/1890950/REAVER/"
    ];


    private List<string> _urlPool;

    private float _chance;
    private UniRandom _random;

    public override void BeginEffect(UniRandom random)
    {
        EventBus.PlayerDied += OnPlayerDied;
        _chance = random.Range(0.5f, 0.85f);
        _random = random;
        _urlPool = new List<string>(s_urlPool);
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
    private void OnDestroy() => EventBus.PlayerDied -= OnPlayerDied;
    private static string CreateGoogleSearchUrl(string query) => $"https://google.com/search?q={query.Replace(' ', '+')}";
}
