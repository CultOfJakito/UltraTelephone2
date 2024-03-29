using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using Configgy;

namespace UltraTelephone.Hydra
{
    public class Jumpscare : MonoBehaviour
    {

        [Configgable("Fun", "Jumpscare Enabled")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private static Jumpscare instance;

        private Image image;
        private RectTransform canvas;
        private AudioSource audioSrc;
        private AudioClip ogClip;

        private Texture2D currentTexture;

        private void Awake()
        {
            instance = this;
            GameObject prefab = UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/JumpscareEngine.prefab");

            if (prefab == null)
            {
                Debug.LogError("Jumpscare prefab not found!");
                return;
            }

            GameObject newJumpscareUI = GameObject.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity);
            canvas = newJumpscareUI.GetComponent<RectTransform>();
            audioSrc = canvas.GetComponent<AudioSource>();
            ogClip = audioSrc.clip;
            image = canvas.GetChild(0).GetComponent<Image>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Jumpscare.Scare(true);
            }
        }

        private float inTime = 0.05f, stayTime = 0.25f, outTime = 0.4f;
        private bool running = false;

        private IEnumerator FlashImage()
        {
            float timer = inTime;
            while (timer > 0.0f && running)
            {
                image.color = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), timer / inTime);
                yield return new WaitForEndOfFrame();
                timer -= Time.deltaTime;
            }

            image.color = Color.white;
            audioSrc.Play();

            timer = stayTime;
            while (timer > 0.0f && running)
            {
                yield return new WaitForEndOfFrame();
                timer -= Time.deltaTime;
            }

            timer = outTime;
            while (timer > 0.0f && running)
            {
                image.color = Color.Lerp(new Color(1.0f, 1.0f, 1.0f, 0.0f), Color.white, timer / outTime);
                yield return new WaitForEndOfFrame();
                timer -= Time.deltaTime;
            }
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

            running = false;
        }

        private static UniRandom rand;

        private bool SetNewTexture()
        {
            rand ??= UniRandom.SessionNext();
            Texture2D newTex = TextureHelper.RandomTextureFromCache(rand);
            if (newTex != null)
            {
                currentTexture = newTex;
                return true;
            }
            return false;
        }

        private void SetNewTexture(Texture2D tex)
        {
            if (tex == null)
            {
                SetNewTexture();
            }
            else
            {
                currentTexture = tex;
            }
        }

        private Sprite TextureToSprite(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        public static void Scare(bool force = false)
        {
            if (!s_enabled.Value)
                return;

            if (instance != null)
                instance.DoScare(force);
        }

        public void DoScare(bool force = false)
        {
            if (running && !force)
                return;

            running = true;
            StopAllCoroutines();
            if (SetNewTexture() && image != null)
            {
                image.sprite = TextureToSprite(currentTexture);
                StartCoroutine(FlashImage());
            }
            else
            {
                running = false;
            }
        }

        public static void ScareWithTexture(Texture2D texture, bool force)
        {
            if (texture == null)
                return;

            if (instance == null)
                return;

            if (instance.running && !force)
                return;

            instance.running = true;
            instance.StopAllCoroutines();
            instance.SetNewTexture(texture);

            if (instance.image != null)
            {
                instance.image.sprite = instance.TextureToSprite(instance.currentTexture);
                instance.StartCoroutine(instance.FlashImage());
            }
            else
            {
                instance.running = false;
            }
        }
    }
}

