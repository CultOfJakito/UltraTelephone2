using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Fun.Casino
{
    public class CasinoGameJuicer : MonoBehaviour
    {
        public AudioSource music;
        public AudioSource fanfareSource;

        public AudioClip winFanfare;
        public AudioClip loseFanfare;
        public AudioClip bigWinFanfare;

        public List<Light> lights;
        public float lightIntensity;

        private bool isPlaying;

        public void ResetJuice()
        {

        }

        public void StartGame()
        {
            if(music != null)
            {
                music.Play();
                MusicManager.Instance.ForceStopMusic();
            }

        }

        public void EndGame()
        {
            if(music != null)
            {
                music.Stop();
                MusicManager.Instance.ForceStartMusic();
            }
        }

        public void BigWin()
        {

            AudioClip clip = bigWinFanfare;
            if(clip == null)
                clip = winFanfare;

           if(clip != null)
                if(fanfareSource != null)
                {
                    fanfareSource.clip = clip;
                    fanfareSource.Play();
                }
        }

        public void Won()
        {
            if(winFanfare != null)
                if(fanfareSource != null)
                {
                    fanfareSource.clip = winFanfare;
                    fanfareSource.Play();
                }

            EndGame();
        }

        public void Lost()
        {
            if(loseFanfare != null)
                if(fanfareSource != null)
                {
                    fanfareSource.clip = loseFanfare;
                    fanfareSource.Play();
                }

            EndGame();
        }

        private int lightIndex = 0;
        private float timeUntilNextLight = 0f;
        private float lightChangeInterval = 0.3f;

        private void Update()
        {
            if (isPlaying)
            {
                float timeSample = Time.time * 2f;

                for (int i = 0; i < lights.Count; i++)
                {
                    float localLightSample = timeSample + (i * 0.5f);
                    lights[i].intensity = Mathf.PingPong(localLightSample, 1) * lightIntensity;
                }
            }
            else
            {
                float timeSample = Time.time * 2f;
                bool lightOn = Mathf.Round(timeSample*20) % 2 == 0;
                for (int i = 0; i < lights.Count; i++)
                {
                    lights[i].intensity = lightIntensity*0.2f;
                    lights[i].enabled = lightOn;
                }
            }

        }

    }
}
