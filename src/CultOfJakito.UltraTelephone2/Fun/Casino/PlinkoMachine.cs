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
        UniRandom trueRandom;

        public AudioSource winSource;
        public AudioSource loseSource;
        public AudioSource jackpotSource;

        private bool isPlaying;

        private List<Transform> triggerRoots;
        private List<Vector3> possiblePositions;

        private void Start()
        {
            random = new UniRandom(new SeedBuilder().WithGlobalSeed().WithSeed("PLINKO!!").WithSceneName().WithSeed(2));
            rb = ball.GetComponent<Rigidbody>();
            ball.name = BALL_NAME;
            triggers = GetComponentsInChildren<PlinkoMachineTrigger>(true);
            ball.AddComponent<PlinkoBall>();

            triggerRoots = new List<Transform>();
            possiblePositions = new List<Vector3>();

            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].Machine = this;
                triggers[i].gameObject.SetActive(false);
                triggerRoots.Add(triggers[i].transform.parent);
                possiblePositions.Add(triggers[i].transform.parent.position);
            }


            int index = 0;
            foreach (var trigger in random.Shuffle(triggerRoots))
            {
                trigger.transform.position = possiblePositions[index];
                ++index;
            }

        }

        public void Play()
        {
            if (isPlaying)
                return;

            bet = betController.BetAmount;
            if (bet <= 0)
                return;

            CasinoManager.Instance.AddChips(-bet);
            isPlaying = true;
            betController.SetLocked(true);
            buttons.SetActive(false);

            ball.transform.position = ballpoint.position;
            ball.SetActive(true);

            for (int i = 0; i < triggers.Length; i++)
            {
                triggers[i].gameObject.SetActive(false);
            }

            //Toss the ball in a random direction
            trueRandom ??= UniRandom.CreateFullRandom();

            Vector3 launch = trueRandom.UnitSphere() * 70f;
            launch.y = Mathf.Abs(launch.y);
            rb.velocity = launch;
        }

        public void SetBet(int type)
        {
            if (isPlaying)
                return;

            typeBet = type;
        }

        private void Update()
        {
            if (!isPlaying)
                return;

            if(rb.velocity.magnitude > 0.5f)
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
                    if(jackpotSource)
                        jackpotSource.Play();

                    //win 10x
                    long winnings = bet * 10;
                    CasinoManager.Instance.Winnings += winnings;
                    CasinoManager.Instance.AddChips(winnings);
                }
                else
                {
                    if(winSource)
                        winSource.Play();

                    long winnings = bet * 2;
                    //Win 2x
                    CasinoManager.Instance.Winnings += winnings;
                    CasinoManager.Instance.AddChips(winnings);
                }
            }
            else
            {
                if(loseSource)
                    loseSource.Play();
            }

            bet = 0;
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

    public class PlinkoBall : MonoBehaviour
    {
        private bool bouncing = true;
        private UniRandom rand;
        private Rigidbody rb;


        private void Awake()
        {
            rand = UniRandom.CreateFullRandom();
            rb = GetComponent<Rigidbody>();
        }

        private float timeLeft = 0.5f;
        private float energy = 20f;

        private void Update()
        {
            energy -= Time.deltaTime * 2.5f;
            timeLeft -= Time.deltaTime;

            if(timeLeft <= 0f)
            {
                bouncing = false;
            }
        }

        private void OnEnable()
        {
            bouncing = true;
            energy = 30f;
            timeLeft = 5f;
        }

        private void OnCollisionEnter(Collision col)
        {
            if (!bouncing)
                return;

            Vector3 normal = col.GetContact(0).normal;
            Vector3 reflect = Vector3.Reflect(rb.velocity.normalized, normal);
            reflect.y = Mathf.Abs(reflect.y);
            reflect.y += 0.12f;
            reflect.x += rand.Range(-0.1f, 0.1f);
            reflect.z += rand.Range(-0.1f, 0.1f);

            rb.velocity = reflect * energy;

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
