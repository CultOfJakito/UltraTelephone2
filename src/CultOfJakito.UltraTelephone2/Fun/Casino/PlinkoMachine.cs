using System;
using System.Collections.Generic;
using System.Text;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    public class PlinkoMachine : MonoBehaviour
    {
        public BetController betController;

        public long bet;
        public int typeBet;

        public const string BALL_NAME = "PLINKOBALL";


        public GameObject ball;
        private Rigidbody rb;
        public GameObject buttons;
        public Transform ballpoint;

        public PlinkoMachineTrigger[] triggers;
        UniRandom random;
        private bool isPlaying;

        private void Start()
        {
            random = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSeed("PLINKO!!").WithSceneName().WithSeed(2));
            rb = ball.GetComponent<Rigidbody>();
            ball.name = BALL_NAME;
            triggers = GetComponentsInChildren<PlinkoMachineTrigger>(true);
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].Machine = this;
                triggers[i].gameObject.SetActive(false);
            }
        }

        public void Play()
        {
            if (isPlaying)
                return;

            bet = betController.BetAmount;
            if (bet <= 0)
                return;

            isPlaying = true;
            betController.SetLocked(true);

            ball.transform.position = ballpoint.position;
            ball.SetActive(true);
            rb.velocity = random.UnitSphere() * 20f;
        }


        private void Update()
        {
            if (!isPlaying)
                return;

            if(rb.velocity.magnitude > 0.1f)
                return;

            rb.velocity = Vector3.zero;
            EvaluateResults();
        }

        public void EvaluateResults()
        {
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].gameObject.SetActive(true);
            }

            this.DoAfterTime(2f, ResetMachine);
        }

        public void BallHitTrigger(int type)
        {
                //Win
            if (type == typeBet)
            {
                //JACKPOT!!!
                if (type == 2)
                {
                    //win 10x
                    long winnings = bet * 10;
                    CasinoManager.Instance.Winnings += winnings;
                    CasinoManager.Instance.Chips += winnings;
                    //jackpot fanfare
                }
                else
                {
                    long winnings = bet * 2;
                    //Win 2x
                    CasinoManager.Instance.Winnings += winnings;
                    CasinoManager.Instance.Chips += winnings;
                    //win fanfare
                }
            }
            else
            {
                //YOU LOSE
                //Lose fanfare   
            }

            this.DoAfterTime(1f, ResetMachine);
        }

        public void ResetMachine()
        {
            bet = 0;
            ball.SetActive(false);

            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].gameObject.SetActive(false);
            }

            isPlaying = false;
            buttons.SetActive(true);
            betController.ResetBet();
            betController.SetLocked(false);
        }

    }

    public class PlinkoMachineTrigger : MonoBehaviour
    {
        public PlinkoMachine Machine;
        public int type = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.name != PlinkoMachine.BALL_NAME)
                return;

            Machine.BallHitTrigger(type);
        }
    }
}
