using CultOfJakito.UltraTelephone2;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun;

public class GlitchyPlush : MonoBehaviour
{
    private static bool s_enemiesLoaded = false;
    private static List<GameObject> s_enemies = new List<GameObject>();
    private static List<string> s_enemyPaths = new List<string>()
    {
        "Assets/Prefabs/Enemies/DroneFlesh.prefab",
        "Assets/Prefabs/Enemies/Projectile Zombie.prefab",
        "Assets/Prefabs/Enemies/Streetcleaner.prefab",
        "Assets/Prefabs/Enemies/Minotaur.prefab",
        "Assets/Prefabs/Enemies/Gabriel.prefab",
        "Assets/Prefabs/Enemies/Stalker.prefab",
        "Assets/Prefabs/Enemies/Zombie.prefab",
        "Assets/Prefabs/Enemies/Idol.prefab",
        "Assets/Prefabs/Enemies/Virtue.prefab",
	    //"Assets/Prefabs/Enemies/Wicked.prefab",
        "Assets/Prefabs/Enemies/SwordsMachineNonboss.prefab",
        "Assets/Prefabs/Enemies/ShotgunHusk.prefab",
        "Assets/Prefabs/Enemies/Flesh Prison 2.prefab",
        "Assets/Prefabs/Enemies/Mannequin.prefab",
        "Assets/Prefabs/Enemies/DroneFleshCamera Variant.prefab",
        "Assets/Prefabs/Enemies/Mindflayer.prefab",
        "Assets/Prefabs/Enemies/Guttertank.prefab",
        "Assets/Prefabs/Enemies/StatueEnemy.prefab",
        "Assets/Prefabs/Enemies/Flesh Prison.prefab",
        "Assets/Prefabs/Enemies/Super Projectile Zombie.prefab",
        "Assets/Prefabs/Enemies/Sisyphus.prefab",
        "Assets/Prefabs/Enemies/SisyphusPrime.prefab",
        "Assets/Prefabs/Enemies/MinosBoss.prefab",
        "Assets/Prefabs/Enemies/Big Johninator.prefab",
	    "Assets/Prefabs/Enemies/Gutterman.prefab",
        "Assets/Prefabs/Enemies/Ferryman.prefab",
        "Assets/Prefabs/Enemies/MinosPrime.prefab",
        "Assets/Prefabs/Enemies/DroneSkull Variant.prefab",
	    "Assets/Prefabs/Enemies/V2 Green Arm Variant.prefab",
        "Assets/Prefabs/Enemies/Drone.prefab",
        "Assets/Prefabs/Enemies/Spider.prefab",
        "Assets/Prefabs/Enemies/Gabriel 2nd Variant.prefab",
        "Assets/Prefabs/Enemies/V2.prefab",
        "Assets/Prefabs/Enemies/Mass.prefab",
        "Assets/Prefabs/Enemies/Puppet.prefab",
        "Assets/Prefabs/Enemies/Turret.prefab",
        "Assets/Prefabs/Enemies/SwordsMachine.prefab",
        "Assets/Prefabs/Enemies/Cancerous Rodent.prefab",
        "Assets/Prefabs/Enemies/Very Cancerous Rodent.prefab",
        "Assets/Prefabs/Enemies/Mandalore.prefab",
        "Assets/Prefabs/Enemies/DroneFlesh.prefab",
        "Assets/Prefabs/Enemies/Projectile Zombie.prefab",
        "Assets/Prefabs/Enemies/Streetcleaner.prefab",
	    "Assets/Prefabs/Enemies/Minotaur.prefab"

    };

    private bool _enemySpawned = false;

    private void LoadEnemies()
    {
        foreach (string enemyPath in s_enemyPaths)
        {
            s_enemies.Add(UT2Assets.GetAsset<GameObject>(enemyPath));
        }

        s_enemiesLoaded = true;
    }

    private void Awake()
    {
        LoadEnemies();
    }

    private void SpawnRandomEnemy()
    {
        int globalSeed = UltraTelephoneTwo.Instance.Random.Seed;
        Vector3 position = NewMovement.Instance.transform.position;
        int sceneSeed = UniRandom.StringToSeed(SceneHelper.CurrentScene);
        int seed = new SeedBuilder()
            .WithGlobalSeed()
            .WithObjectHash(position)
            .WithSceneName()
            .GetSeed();
        UniRandom rng = new UniRandom(seed);

        Instantiate(rng.SelectRandom(s_enemies), this.transform.position, Quaternion.identity);
        _enemySpawned = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(_enemySpawned)
            return;
        SpawnRandomEnemy();
    }
}
