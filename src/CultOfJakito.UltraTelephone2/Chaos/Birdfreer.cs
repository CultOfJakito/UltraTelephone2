using Configgy;
using CultOfJakito.UltraTelephone2;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using UnityEngine;

namespace UltraTelephone.Hydra
{
    [RegisterChaosEffect]
    public class BirdFreer : ChaosEffect
    {
        [Configgable("Hydra/Chaos/Freebird", "Free Neco Arc")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        [Configgable("Hydra/Chaos/Freebird")]
        private static ConfigInputField<int> minBirdAmount = new ConfigInputField<int>(1, (v) => { return v >= 1 && v <= maxBirdAmount.Value; });

        [Configgable("Hydra/Chaos/Freebird")]
        private static ConfigInputField<int> maxBirdAmount = new ConfigInputField<int>(18, (v) => { return v >= 1 && v >= minBirdAmount.Value; });

        private GameObject freeBird;

        private List<GameObject> activeBirds = new List<GameObject>();


        private int currentBirdAmount = 1;

        private float timeTillNextBird = 0.0f;


        public override void Dispose()
        {
            KillAll();
            base.Dispose();
        }

        private void KillAll()
        {
            for(int i = 0; i < activeBirds.Count; i++)
            {
                GameObject bird = activeBirds[i];
                if (bird == null)
                    continue;

                activeBirds[i] = null;
                Destroy(bird.gameObject);
            }
        }

        private void Update()
        {
            if(timeTillNextBird > 0f)
            {
                timeTillNextBird = Mathf.Max(0f, timeTillNextBird - Time.deltaTime);
                return;
            }

            if (activeBirds.Count < currentBirdAmount)
                return;

            timeTillNextBird = UnityEngine.Random.Range(3.0f, 17.0f);
            SpawnBird();
        }

        private void SpawnBird()
        {
            if (freeBird != null)
            {
                Vector3 spawnPos = NewMovement.Instance.transform.position;
                Vector3 randomOffset = UnityEngine.Random.insideUnitSphere;
                spawnPos += randomOffset * 2.1f;
                GameObject newbird = GameObject.Instantiate<GameObject>(freeBird, spawnPos, Quaternion.identity);
                activeBirds.Add(newbird);
            }
        }

        public override void BeginEffect(UniRandom random)
        {
            if(freeBird == null)
                freeBird = UT2Assets.GetAsset<GameObject>("FreeBird");

            currentBirdAmount = random.Range(minBirdAmount.Value, maxBirdAmount.Value);
        }

        public override int GetEffectCost() => 1;
        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && base.CanBeginEffect(ctx);
    }

    public class FreedBird : MonoBehaviour
    {

        public float maxSpeed = 150.0f, minSpeed = 50.0f;
        private float speed = 0.0f;

        private Transform player;
        //private Rigidbody rb;
        private AudioSource clipPlayer;

        private void Awake()
        {
            GeneralSettings.EnableCopyrightedMusic.OnValueChanged += SetAudioEnabled;
        }

        private void SetAudioEnabled(bool enabled)
        {
            if(clipPlayer != null)
                clipPlayer.enabled = enabled;
        }

        private bool Birth()
        {
            player = CameraController.Instance.transform;
            clipPlayer = GetComponent<AudioSource>();

            if (TryGetComponent<Rigidbody>(out Rigidbody rb)) //meh
            {
                Destroy(rb);
            }

            if (clipPlayer == null || player == null)
            {
                return false;
            }

            float stereoPan = UnityEngine.Random.Range(-1.0f, 1.0f);
            clipPlayer.enabled = GeneralSettings.EnableCopyrightedMusic.Value;
            clipPlayer.panStereo = stereoPan;
            speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
            return true;
        }

        private void Start()
        {
            if (!Birth())
            {
                Kill();
            }
        }

        private float drag = 0.75f;

        private Vector3 velocity;

        //Shiny new fake physics orbit :3
        private void Update()
        {
            if (!clipPlayer.isPlaying || player == null)
            {
                Kill();
            }

            Vector3 newVelo = velocity - (velocity * Time.deltaTime * drag);

            Vector3 targetDirection = player.position - transform.position;

            newVelo += (targetDirection.normalized * speed * Time.deltaTime);

            velocity = newVelo;

            transform.position += velocity * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection, Vector3.up), 15.0f * Time.deltaTime);
        }

        public void Kill()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            GeneralSettings.EnableCopyrightedMusic.OnValueChanged -= SetAudioEnabled;
        }
    }


}
