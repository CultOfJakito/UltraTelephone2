using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    public class NamedEnemies : ChaosEffect
    {
        [Configgable("Chaos/Effects/Named Enemies", "Enemies Are Named")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Chaos/Effects/Named Enemies", "Use Steam Friendlist")]
        private static ConfigToggle s_useSteamFriends = new ConfigToggle(true);

        private static bool s_effectActive = false;
        private static UniRandom s_rng;

        private static List<string> s_steamFriendNames;
        private static bool s_steamConnected;

        private static List<string> GetNamePool()
        {
            if (s_useSteamFriends.Value && s_steamConnected && s_steamFriendNames != null && s_steamFriendNames.Count > 0)
            {
                return s_steamFriendNames;
            }

            return UT2TextFiles.EnemyNamesFile.TextList;
        }

        public override void BeginEffect(UniRandom random)
        {
            s_rng = random;
            s_effectActive = true;
            s_steamConnected = SteamClient.IsLoggedOn;

            s_steamFriendNames ??= new List<string>();

            if (s_steamConnected && s_useSteamFriends.Value)
                CacheFriendNames();
        }

        private void CacheFriendNames()
        {
            bool retryCache = false;

            foreach (var friend in SteamFriends.GetFriends())
            {
                string friendName = friend.Name;

                //Handle failure to get name
                if (friendName == "[unknown]")
                    retryCache = true;
                else if(!s_steamFriendNames.Contains(friendName))
                    s_steamFriendNames.Add(friendName);
            }

            if (retryCache)
                Invoke(nameof(CacheFriendNames), 1.5f);
        }

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);

        private static string GetRandomName()
        {
            string name = s_rng.SelectRandom(GetNamePool());
            name = SillyNames.SillifyName(name);
            return name;
        }

        public override int GetEffectCost() => 1;

        protected override void OnDestroy() => s_effectActive = false;

        [HarmonyPatch(typeof(SeasonalHats), nameof(SeasonalHats.Start)), HarmonyPostfix]
        private static void OnEnemySpawned(SeasonalHats __instance)
        {
            if (!s_effectActive || !s_enabled.Value)
                return;

            EnemyIdentifier enemy = __instance.GetComponentInParent<EnemyIdentifier>();
            if (enemy == null) //only do this on enemies.
                return;

            string name = GetRandomName();
            enemy.overrideFullName = name;

            if(enemy.GetComponentInChildren<BossHealthBar>())
            {
                BossHealthBar bossbar = enemy.GetComponentInChildren<BossHealthBar>();
                bossbar.bossName = name;

                int bossBarid = bossbar.GetInstanceID();
                if(BossBarManager.Instance != null)
                {
                    if (BossBarManager.Instance.bossBars.ContainsKey(bossBarid))
                    {
                        BossHealthBarTemplate template = BossBarManager.Instance.bossBars[bossBarid];
                        for(int i=0;i<template.textInstances.Length;i++)
                        {
                            template.textInstances[i].text = name;
                        }
                    }
                }
            }

            GameObject nametag = GameObject.Instantiate(HydraAssets.Nametag, __instance.transform);
            nametag.transform.localPosition = new Vector3(0f, 0f, 0f);
            nametag.transform.localRotation = Quaternion.identity;
            nametag.transform.localScale = Vector3.one;

            Text txt = nametag.GetComponentInChildren<Text>();
            txt.text = name;
        }
    }
}
