using CultOfJakito.UltraTelephone2.Fun.Coin;
using CultOfJakito.UltraTelephone2.Fun.FakePBank;
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

            betController.SetLocked(true);
            bet = betController.BetAmount;

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

            for (int i = 0; i < reels.Length; i++)
            {
                int index = i;
                this.DoAfterTime(spinStopDelay * (i + 1), () =>
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

            float totalDelay = (spinStopDelay * reels.Length) + spinStopDelay;

            this.DoAfterTime(totalDelay, () =>
            {
                EvaluateResults();
            });
        }

        private SlotMachineSymbol winningSymbol;

        private void EvaluateResults()
        {
            SlotMachineSymbol[] symbols = new SlotMachineSymbol[reels.Length];

            for (int i = 0; i < reels.Length; i++)
            {
                symbols[i] = reels[i].GetSymbol();
                Debug.Log(symbols[i]);
            }


            float delay = 1.2f;

            if (symbols.Distinct().Count() == 1)
            {
                delay = 10f;
                Debug.Log("WINNER");
                winningSymbol = symbols[0];
                PlayerWon();
            }
            else
            {
                Debug.Log("LOSER AHAHAHAHA");
                PlayerLost();
            }

            this.DoAfterTime(delay, () =>
            {
                ResetMachine();
            });
        }

        private void PlayerWon()
        {
            if (lightsAnimator)
                lightsAnimator.Play("Winner", 0, 0f);

            switch (winningSymbol)
            {
                //Coin fountain
                case SlotMachineSymbol.Coin:
                    //1.25x bet and coin fountain
                    CoinFountain();
                    FakeBank.AddMoney((long)(bet * 1.25f));
                    break;

                case SlotMachineSymbol.Maurice:
                case SlotMachineSymbol.Heckteck:
                case SlotMachineSymbol.Skull:
                    FakeBank.AddMoney(bet * 2);
                    break;

                    //JACKPOT!!!
                case SlotMachineSymbol.GoldenP:
                    //10x bet and three coin fountains
                    CoinFountain();
                    CoinFountain().transform.position = coinFountainLocation.position + coinFountainLocation.rotation * new Vector3(1,0,0);
                    CoinFountain().transform.position = coinFountainLocation.position + coinFountainLocation.rotation * new Vector3(-1,0,0);
                    FakeBank.AddMoney(bet * 10);
                    break;
            }


            switch (winningSymbol)
            {
                case SlotMachineSymbol.Coin:
                case SlotMachineSymbol.Maurice:
                case SlotMachineSymbol.Heckteck:
                case SlotMachineSymbol.Skull:
                    if (winAudioSource != null)
                        winAudioSource.Play();
                    break;
                case SlotMachineSymbol.GoldenP:
                    if (jackpotAudioSource != null)
                        jackpotAudioSource.Play();
                    break;
            }

            bet = 0;
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

        const float SPIN_SPEED = 740f;
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
            CurrentSide = Mathf.RoundToInt(rotation / (SliceAngle())) % SIDE_COUNT;
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
