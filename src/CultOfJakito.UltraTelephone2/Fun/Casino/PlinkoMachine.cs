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

        public GameObject[] triggers;

        private bool isPlaying;

        private void Start()
        {
            rb = ball.GetComponent<Rigidbody>();
            ball.name = BALL_NAME;
        }

        public void Play()
        {
            if (isPlaying)
                return;

            isPlaying = true;
            betController.SetLocked(true);
            bet = betController.BetAmount;

            ball.transform.position = ballpoint.position;
            ball.SetActive(true);
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
                triggers[i].SetActive(true);
            }
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
            betController.SetLocked(false);
            ball.SetActive(false);
            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].SetActive(false);
            }
            isPlaying = false;
            buttons.SetActive(true);
        }

    }

    public class PlinkoMachineTrigger : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {

        }
    }
}
