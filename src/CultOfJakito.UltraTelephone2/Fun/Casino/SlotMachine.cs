using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Fun.Coin;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    public class SlotMachine : MonoBehaviour
    {
        public SlotMachineReel[] reels;
        public Animator leverAnimator;
        public Animator lightsAnimator;
        public BetController betController;

        public Transform coinFountainLocation;
        public ScreenZone screenZone;
        public GameObject buttons;

        public bool isRigged;

        public float spinSpeed = 1f;
        public float spinStartDelay = 0.35f;
        public float spinTime = 2f;
        public float spinStopDelay = 0.7f;

        public AudioSource winAudioSource;
        public AudioSource loseAudioSource;
        public AudioSource jackpotAudioSource;

        private UniRandom random;
        public string seed;

        private SlotMachineState state = SlotMachineState.Idle;

        private long bet;

        private void Awake()
        {
            random = new UniRandom(new SeedBuilder()
                .WithGlobalSeed()
                .WithSeed(transform.position)
                .WithSeed(seed));

            isRigged = random.Bool();
        }

        public void Spin()
        {
            if (state != SlotMachineState.Idle)
                return;

            bet = betController.BetAmount;

            if (bet <= 0)
                return;

            betController.SetLocked(true);
            CasinoManager.Instance.AddChips(-bet);

            //Disable the buttons.
            buttons.SetActive(false);
            leverAnimator.Play("PullLever", 0, 0f);

            if(lightsAnimator)
                lightsAnimator.Play("Playing", 0, 0f);

            StartSpinning();
        }

        private void StartSpinning()
        {
            state = SlotMachineState.Spinning;
            for (int i = 0; i < reels.Length; i++)
            {
                int index = i;
                this.DoAfterTime(spinStartDelay * (i + 1), () =>
                {
                    reels[index].StartSpinning();

                    if (index == reels.Length - 1)
                    {
                        //last reel
                        this.DoAfterTime(spinTime, () =>
                        {
                            StopSpinning();
                        });
                    }
                });
            }
        }

        public void StopSpinning()
        {
            state = SlotMachineState.Stopping;
            int[] results = new int[reels.Length];

            float totalDelay = 0f;

            for (int i = 0; i < reels.Length; i++)
            {
                //cache index 
                int index = i;
                float localDelay = (spinStopDelay * (i + 1)) + random.Range(0f, 1f);
                totalDelay += localDelay;

                this.DoAfterTime(localDelay, () =>
                {
                    reels[index].Lock();
                    results[index] = reels[index].CurrentSide;

                    if (index == reels.Length - 1)
                    {
                        //OH NO THE PLAYER IS ABOUT TO WIN... not anymore .....
                        if (isRigged && results.Distinct().Count() == 1 && reels[index].PredictStop() == results[0])
                        {
                            //offset by 1 to make sure the player doesn't win
                            reels[index].OffsetSide(1);
                            results[index] = reels[index].CurrentSide;
                        }
                    }
                });

            }

            totalDelay += 0.25f;

            this.DoAfterTime(totalDelay, () =>
            {
                EvaluateResults();
            });
        }

        private float winningsMultiplier;
        private bool coinFountain;

        private SlotMachineSymbol winningSymbol;
        SlotMachineSymbol[] symbolsResults;

        private Dictionary<SlotMachineSymbol, int> counts = new Dictionary<SlotMachineSymbol, int>();

        private void EvaluateResults()
        {
            symbolsResults = new SlotMachineSymbol[reels.Length];

            counts.Clear();

            counts.Add(SlotMachineSymbol.Coin, 0);
            counts.Add(SlotMachineSymbol.Maurice, 0);
            counts.Add(SlotMachineSymbol.Heckteck, 0);
            counts.Add(SlotMachineSymbol.Skull, 0);
            counts.Add(SlotMachineSymbol.GoldenP, 0);


            for (int i = 0; i < reels.Length; i++)
            {
                symbolsResults[i] = reels[i].GetSymbol();
                ++counts[symbolsResults[i]];
                Debug.Log(symbolsResults[i]);
            }

            //no win at all
            if(symbolsResults.Distinct().Count() == 3)
            {
                PlayerLost();
                return;
            }


            if (counts[SlotMachineSymbol.GoldenP] == 3)
            {
                //JACKPOT!
                if(jackpotAudioSource)
                    jackpotAudioSource.Play();

                CoinFountain();
                CoinFountain().transform.position = coinFountainLocation.position + coinFountainLocation.rotation * new Vector3(1, 0, 0);
                CoinFountain().transform.position = coinFountainLocation.position + coinFountainLocation.rotation * new Vector3(-1, 0, 0);
                CasinoManager.Instance.AddChips(bet * 10);
                bet = 0;
                this.DoAfterTime(10f, ResetMachine);
                return;
            }


            if (counts[SlotMachineSymbol.Coin] == 3)
            {
                if (winAudioSource)
                    winAudioSource.Play();

                CoinFountain();
                CoinFountain().transform.position = coinFountainLocation.position + coinFountainLocation.rotation * new Vector3(1, 0, 0);
                CoinFountain().transform.position = coinFountainLocation.position + coinFountainLocation.rotation * new Vector3(-1, 0, 0);
                CasinoManager.Instance.AddChips((long)(bet * 1.7777));
                bet = 0;

                this.DoAfterTime(10f, ResetMachine);
                return;
            }

            if (counts[SlotMachineSymbol.Maurice] == 3)
            {
                if (winAudioSource)
                    winAudioSource.Play();

                CasinoManager.Instance.AddChips(bet * 2);
                bet = 0;
                this.DoAfterTime(3f, ResetMachine);
                return;
            }

            if (counts[SlotMachineSymbol.Heckteck] == 3)
            {
                if (winAudioSource)
                    winAudioSource.Play();

                CasinoManager.Instance.AddChips((long)(bet * 2.5));
                bet = 0;
                this.DoAfterTime(3f, ResetMachine);
                return;
            }

            if (counts[SlotMachineSymbol.Skull] == 3)
            {
                if (winAudioSource)
                    winAudioSource.Play();

                CasinoManager.Instance.AddChips(bet * 3);

                //Mind flayer :3
                UkPrefabs.MindFlayer.LoadObjectAsync((a, g) =>
                {

                    if(a == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        GameObject.Instantiate(g, coinFountainLocation.position, Quaternion.identity);
                    }
                });

                bet = 0;
                this.DoAfterTime(10f, ResetMachine);
                return;
            }

            if (counts[SlotMachineSymbol.GoldenP] == 2)
            {
                if (winAudioSource)
                    winAudioSource.Play();

                CasinoManager.Instance.AddChips(bet);
                bet = 0;
                this.DoAfterTime(3f, ResetMachine);
                return;
            }

            if (counts[SlotMachineSymbol.Heckteck] == 2 || counts[SlotMachineSymbol.Maurice] == 2 || counts[SlotMachineSymbol.Skull] == 2)
            {
                if (winAudioSource)
                    winAudioSource.Play();

                CasinoManager.Instance.AddChips((long)(bet * 1.25f));
                bet = 0;
                this.DoAfterTime(3f, ResetMachine);
                return;
            }

            if (counts[SlotMachineSymbol.Coin] == 2)
            {
                if (winAudioSource)
                    winAudioSource.Play();

                CoinFountain();
                CasinoManager.Instance.AddChips(bet);
                bet = 0;
                this.DoAfterTime(2f, ResetMachine);
                return;
            }

            PlayerLost();
        }


        private Transform CoinFountain()
        {
            GameObject fountain = new GameObject("fountain");
            fountain.transform.position = coinFountainLocation.position;
            fountain.AddComponent<CoinFountain>();
            RemoveOnTime rot1 = fountain.AddComponent<RemoveOnTime>();
            rot1.time = 15f;
            return fountain.transform;
        }

        private void PlayerLost()
        {
            if (lightsAnimator)
                lightsAnimator.Play("Loser", 0, 0f);

            //Boo womp
            if (loseAudioSource != null)
                loseAudioSource.Play();

            bet = 0;
            this.DoAfterTime(1.2f, ResetMachine);
        }

        private void ResetMachine()
        {
            state = SlotMachineState.Idle;
            buttons.SetActive(true);
            betController.ResetBet();
            betController.SetLocked(false);
        }
    }

    public enum SlotMachineState
    {
        Idle,
        Spinning,
        Stopping,
        Rewarding
    }

    public enum SlotMachineSymbol
    {
        Coin, //2 of these
        Maurice, //2 of these
        Heckteck, //2 of these
        Skull, //2 of these
        GoldenP, //1 of these
    }

    //Coin
    //Maurice
    //Heckteck
    //Skull
    //GoldenP
    //Coin
    //Maurice
    //Heckteck
    //Skull

    public class SlotMachineReel : MonoBehaviour
    {
        const int SIDE_COUNT = 9;
        public int CurrentSide = 0;

        const float SPIN_SPEED = 960f;
        private bool spinning;
        float rotation;

        public SlotMachineSymbol GetSymbol()
        {
            switch (CurrentSide)
            {
                case 0:
                case 4:
                    return SlotMachineSymbol.Coin;
                case 1:
                case 6:
                    return SlotMachineSymbol.Skull;
                case 2:
                case 7:
                    return SlotMachineSymbol.Heckteck;
                case 3:
                case 8:
                    return SlotMachineSymbol.Maurice;
                case 5:
                    return SlotMachineSymbol.GoldenP;
            }

            //huh?
            return SlotMachineSymbol.Coin;
        }

        public void StartSpinning()
        {
            spinning = true;
        }

        public void OffsetRotation(float amount)
        {
            rotation += amount;

            if(Mathf.Abs(rotation) > 360f)
                rotation = rotation % 360f;

            transform.localRotation = Quaternion.Euler(0, 0f, rotation);
            UpdateCurrentSide();
        }

        public void SetRotation(float amount)
        {
            rotation = amount;
            if (Mathf.Abs(rotation) > 360f)
                rotation = rotation % 360f;


            transform.localRotation = Quaternion.Euler(0, 0f, rotation);
            UpdateCurrentSide();
        }

        private void UpdateCurrentSide()
        {
            CurrentSide = Mathf.RoundToInt(Mathf.Abs(rotation) / (SliceAngle())) % SIDE_COUNT;
        }


        private float SliceAngle()
        {
            return 360f / (float)SIDE_COUNT;
        }

        public void OffsetSide(int offset)
        {
            rotation += offset * (SliceAngle());

            if (Mathf.Abs(rotation) > 360f)
                rotation = rotation % 360f;


            transform.localRotation = Quaternion.Euler(0, 0f, rotation);
            UpdateCurrentSide();
        }

        public void Lock()
        {
            spinning = false;
            UpdateCurrentSide();
            rotation = CurrentSide * ((float)360/(float)SIDE_COUNT);
            SetRotation(rotation);
        }

        public int PredictStop()
        {
            return Mathf.RoundToInt(rotation / ((float)360 / (float)SIDE_COUNT)) % SIDE_COUNT;
        }

        private void Update()
        {
            if (spinning)
            {
                //minus to spin the other way
                OffsetRotation(-(SPIN_SPEED * Time.deltaTime));
            }
        }
    }
}
