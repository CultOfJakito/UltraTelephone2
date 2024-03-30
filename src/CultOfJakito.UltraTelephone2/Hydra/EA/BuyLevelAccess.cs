using System.Reflection;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Hydra.FakePBank;
using CultOfJakito.UltraTelephone2.LevelInjection;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Hydra.EA
{
    [RegisterLevelInjector]
    public class BuyLevelAccess : MonoBehaviour, IBuyable, ILevelInjector
    {
        private GameObject barricade;
        private GameObject breakFX;
        private Text costText;
        private Text levelNameText;
        private Text purchaseDescriptionText;

        private GameObject instancedBarricade;

        private ShopZone shop;
        private Button button;
        private ShopButton shopButton;

        public string GetBuyableID()
        {
            return $"buyable.level.{SceneHelper.CurrentScene}";
        }

        private long? cost;

        public long GetCost()
        {
            if (cost != null)
                return cost.Value;

            string sceneName = SceneHelper.CurrentScene;

            if(sceneName.Contains("Level 0-"))
            {
                //its FREE!!
                return 0L;
            }

            //Parse numbers from level name for price :3
            long basePrice = 1000;
            long finalPrice = basePrice;

            List<int> levelNumbers = new List<int>();

            for(int i = 0; i < sceneName.Length; i++)
            {
                char c = sceneName[i];

                if (char.IsDigit(c))
                    levelNumbers.Add(int.Parse(c.ToString()));
                else
                {
                    if(c == 'P' && i + 2 < sceneName.Length)
                    {
                        //Prime sanctum... BIG MONEY$$$$
                        if (sceneName[i+1] == '-' && char.IsDigit(sceneName[i + 2]))
                        {
                            levelNumbers.Add(int.Parse(sceneName[i+2].ToString()) * 100);
                        }
                    }

                    //Secret level... MORE BIG MONEY$$$$
                    if(c == 'S' && i-1>=0 && sceneName[i-1] == '-')
                    {
                        if(i-2 >= 0 && char.IsDigit(sceneName[i - 2]))
                        {
                            levelNumbers.Add(int.Parse(sceneName[i-2].ToString()) * 10);
                        }
                    }
                }
            }

            if(levelNumbers.Count > 0)
            {
                long newCost = 10L;
                foreach (int cost in levelNumbers)
                {
                     newCost *= cost * 10;
                }

                finalPrice += newCost;
            }
            else
            {
                //Randomly generate a price based on the name of the level.
                int levelSeed = UniRandom.StringToSeed(SceneHelper.CurrentScene);
                int globalSeed = UltraTelephoneTwo.Instance.Random.Seed;
                int finalSeed = globalSeed ^ levelSeed;
                finalPrice += new UniRandom(finalSeed).Range(4, 10) * 1000L;
            }

            cost = finalPrice;
            return cost.Value;
        }

        private static FieldInfo s_shopZoneOnEnterZoneField = typeof(ShopZone).GetField("onEnterZone", BindingFlags.NonPublic | BindingFlags.Instance);

        private static UnityEvent GetShopZoneOnEnterZone(ShopZone shop)
        {
            return (UnityEvent)s_shopZoneOnEnterZoneField.GetValue(shop);
        }

        private void Start()
        {
            if(instancedBarricade == null)
            {
                //Destroy self if the barricade is not found.
                UnityEngine.Object.Destroy(this);
                return;
            }

            shop = instancedBarricade.GetComponentInChildren<ShopZone>(true);
            GetShopZoneOnEnterZone(shop).AddListener(UpdateButton);

            shopButton = instancedBarricade.GetComponentInChildren<ShopButton>(true);
            button = instancedBarricade.GetComponentInChildren<Button>(true);
            button.onClick.AddListener(Buy);

            barricade = instancedBarricade.transform.LocateComponentInChildren<Transform>("Barricade").gameObject;

            costText = instancedBarricade.transform.LocateComponentInChildren<Text>("Text_Cost");
            levelNameText = instancedBarricade.transform.LocateComponentInChildren<Text>("Text_LevelName");
            purchaseDescriptionText = instancedBarricade.transform.LocateComponentInChildren<Text>("Text_Description");

            bool isBought = BuyablesManager.IsBought(GetBuyableID());

            if(isBought)
            {
                instancedBarricade.gameObject.SetActive(false);
                return;
            }

            long cost = GetCost();

            if (cost > 0)
                costText.text = FakeBank.PString(cost);
            else
                costText.text = "<color=orange>-- FREE --</color>";
            levelNameText.text = SceneHelper.CurrentScene;
            //purchaseDescriptionText.text 
            UpdateButton();
        }

        private void UpdateButton()
        {
            ShopButton shopButton = instancedBarricade.GetComponentInChildren<ShopButton>(true);
            shopButton.failure = FakeBank.GetCurrentMoney() < GetCost();
        }

        public void Buy()
        {
            long money = FakeBank.GetCurrentMoney();
            if (money < GetCost())
                return;

            long newMoney = money - GetCost();
            BuyablesManager.Bought(this);
            FakeBank.SetMoney(newMoney);
            BreakBarricade();
            HudMessageReceiver.Instance.SendHudMessage($"THANK YOU FOR YOUR PURCHASE!");
        }

        //Breaks the barricade with a cool effect
        private void BreakBarricade()
        {
            if (breakFX == null)
                breakFX = UkPrefabs.BreakParticleMetal.GetObject();

            if(breakFX)
                GameObject.Instantiate(breakFX, barricade.transform.position, Quaternion.identity);

            instancedBarricade.gameObject.SetActive(false);
            shop.UpdatePlayerState(false);
        }

        public void OnLevelLoaded(string sceneName)
        {
            //Find first room.
            //Spawn the barricade.
            FirstRoomPrefab firstRoom = FindObjectOfType<FirstRoomPrefab>();
            if (firstRoom == null)
                return;

            instancedBarricade = Instantiate(HydraAssets.BuyLevelAccessBarricade, firstRoom.transform);
        }
    }
}
