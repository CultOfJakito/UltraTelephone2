using CultOfJakito.UltraTelephone2.Data;
using CultOfJakito.UltraTelephone2.Fun.FakePBank;
using CultOfJakito.UltraTelephone2.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Fun.Captcha
{
    public class ImageSelectCaptcha : Captcha
    {
        public Button verifyButton;
        public GameObject loadingCircle;
        public TMP_Text promptText;

        public ImageSelectCaptchaButton[] buttons;

        UniRandom random;

        int correctImageAmount = 0;

        private void Awake()
        {
            random = new UniRandom(new SeedBuilder()
                .WithGlobalSeed()
                .WithSceneName()
                .WithSeed(nameof(ImageSelectCaptcha)));

            verifyButton.onClick.AddListener(VerifyButton_OnClick);
        }

        public override void ShowCaptcha()
        {
            correctImageAmount = random.Range(0, 10);

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].master = this;
                buttons[i].SetClicked(false);
                buttons[i].SetTexture(TextureHelper.RandomTextureFromCache(random));
            }

            loadingCircle.SetActive(false);
            verifyButton.interactable = true;
            verifyButton.gameObject.SetActive(true);

            string randomWord = random.SelectRandom(UT2TextFiles.S_WordList10k.TextList);
            promptText.text = "Select all images containing: " + randomWord;

            gameObject.SetActive(true);
        }

        public void OnImageClicked(ImageSelectCaptchaButton button)
        {
            float delay = random.Range(0.1f, 1.2f);

            //laggy on purpose :3c
            this.DoAfterTimeUnscaled(delay, () =>
            {
                if(random.Bool())
                {
                    button.SetClicked(true);
                }
                else
                {
                    button.SetClicked(true);

                    //randomly unclick the button after a short delay to make it really annoying and unresponsive
                    this.DoAfterTimeUnscaled(delay * 0.25f, () =>
                    {
                        button.SetClicked(false);
                        if (random.Chance(0.2f))
                        {
                            //randomly change the texture to confuse the player
                            button.SetTexture(TextureHelper.RandomTextureFromCache(random));
                        }
                    });
                }
            });
        }

        private void VerifyButton_OnClick()
        {
            int clicked = buttons.Count(b => b.clicked);
            int distanceFromCorrect = Mathf.Abs(clicked - correctImageAmount);
            float score = Mathf.Clamp01(1f - ((float)distanceFromCorrect / 9f));

            float delay = random.Range(0.4f, 3f);
            loadingCircle.SetActive(true);
            verifyButton.interactable = false;
            verifyButton.gameObject.SetActive(false);


            this.DoAfterTimeUnscaled(delay, () =>
            {
                if (random.Chance(score))
                {
                    m_manager.CaptchaDone();
                }
                else
                {
                    m_manager.CaptchaFailed();
                }
            });
        }

        public override void HideCaptcha()
        {
            gameObject.SetActive(false);
        }
    }

    public class ImageSelectCaptchaButton : MonoBehaviour
    {
        public RawImage Image;
        public Button Button;
        public ImageSelectCaptcha master;
        public bool clicked;

        UniRandom random;

        private void Awake()
        {
            random = UniRandom.CreateFullRandom();
            Button.onClick.AddListener(Button_OnClick);
        }

        public void SetTexture(Texture2D tex)
        {
            Image.texture = tex;
        }

        private void Button_OnClick()
        {
            if (!clicked)
                master.OnImageClicked(this);
        }

        public void SetClicked(bool isClicked)
        {
            this.clicked = isClicked;
            transform.localScale = isClicked ? new Vector3(0.85f, 0.85f, 0.85f) : Vector3.one;
        }
    }
}
