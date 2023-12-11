using System.Collections.Concurrent;
using BepInEx.Logging;
using CultOfJakito.UltraTelephone2.DependencyInjection;

namespace CultOfJakito.UltraTelephone2.Chaos;

internal class OpenUrlOnDeath : ChaosEffect {
	private static readonly string[] s_urlPool = [
		CreateGoogleSearchUrl("why am i so bad at video games"),
		CreateGoogleSearchUrl("ultrakill trainer free download"),
		CreateGoogleSearchUrl("ultrakill epic swag guide"),
		CreateGoogleSearchUrl("suicide hotline"),
		CreateGoogleSearchUrl("suicide hotline free download"),
		"youtube.com/playlist?list=PLtr1CuIZfdMAwqqRa29SrZhuwzPyKOGqw", // herbmessiah ultrakill guides
		"store.steampowered.com/app/1890950/REAVER/"
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
			_urlPool.RemoveAt(index);
			UnityEngine.Application.OpenURL(url);
		}
	}

	private void OnDestroy() {
		EventBus.PlayerDied -= OnPlayerDied;
	}

	private static string CreateGoogleSearchUrl(string query) {
		return $"google.com/search?q={query.Replace(' ', '+')}";
	}
}
