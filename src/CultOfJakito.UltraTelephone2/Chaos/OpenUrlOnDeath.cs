using System.Collections.Concurrent;
using BepInEx.Logging;
using CultOfJakito.UltraTelephone2.DependencyInjection;

namespace CultOfJakito.UltraTelephone2.Chaos;

internal class OpenUrlOnDeath : ChaosEffect {
	private static readonly string[] s_urlPool = [
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

	[Inject]
	public ManualLogSource Logger { get; set; }

	private List<string> _urlPool;

	private double _chance = 0;
	private Random _random;

	public override void BeginEffect(Random random) {
		EventBus.PlayerDied += OnPlayerDied;
		_chance = random.NextDouble(0.5, 0.85);
		_random = random;
		_urlPool = new List<string>(s_urlPool);
		Logger.LogInfo("Chance to open URL is " + _chance);
	}

	private void OnPlayerDied() {
		if(_random.NextDouble() <= _chance) {
			if(_urlPool.Count <= 0) {
				_urlPool.AddRange(s_urlPool);
			}

			int index = _random.Next(_urlPool.Count);
			string url = _urlPool[index];
			Logger.LogInfo($"Opening URL {url}");
			_urlPool.RemoveAt(index);
			UnityEngine.Application.OpenURL(url);
		}
	}

    public override int GetEffectCost()
    {
		return 1;
    }

    private void OnDestroy() {
		EventBus.PlayerDied -= OnPlayerDied;
	}

	private static string CreateGoogleSearchUrl(string query) {
		return $"https://google.com/search?q={query.Replace(' ', '+')}";
	}
}
