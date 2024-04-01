using System.Collections;
using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Util;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.Fun
{
    [HarmonyPatch]
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

        [HarmonyPatch(typeof(CanvasController), nameof(CanvasController.Awake)), HarmonyPostfix]
        private static void CreateJumpscare()
        {
            if (!s_enabled.Value || instance != null)
            {
                return;
            }

            new GameObject("awesome gameobject").gameObject.gameObject.gameObject.gameObject.gameObject.gameObject.gameObject.AddComponent<Jumpscare>();
        }

        private void Awake()
        {
            instance = this;
            GameObject prefab = UT2Assets.GetAsset<GameObject>("Assets/Telephone 2/Misc/Prefabs/JumpscareEngine.prefab");

            if (prefab == null)
            {
                Debug.LogError("Jumpscare prefab not found!");
                return;
            }

            GameObject newJumpscareUI = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newJumpscareUI.transform.parent = transform;
            canvas = newJumpscareUI.GetComponent<RectTransform>();
            audioSrc = canvas.GetComponent<AudioSource>();
            ogClip = audioSrc.clip;
            image = canvas.GetChild(0).GetComponent<Image>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Scare(true);
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
            Debug.Log($"Doscare: {running} {force}");
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

        private static string s_jumpscarePicsPath => Path.Combine(UT2Paths.TextureFolder, "Jumpscare");

        private static void ValidateBuiltInImage(string filePath, byte[] data)
        {
            if (File.Exists(filePath))
                return;

            File.WriteAllBytes(filePath, data);
        }

        public static void ValidateFiles()
        {
            //Only unpack if the directory doesn't exist since the user may have added their own images and deleted the packed ones
            if (!Directory.Exists(s_jumpscarePicsPath))
            {
                Directory.CreateDirectory(s_jumpscarePicsPath);

                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "bro.jpg"), Properties.Resources.bro);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "CatExperiencee.PNG"), Properties.Resources.CatExperiencee);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "chartreused.png"), Properties.Resources.chartreused);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "chees1.jpg"), Properties.Resources.chees1);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "coconut.png"), Properties.Resources.coconut);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "ComeHere.PNG"), Properties.Resources.ComeHere);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "cover2 (4).jpg"), Properties.Resources.cover2__4_);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "crydeath.jpg"), Properties.Resources.crydeath);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "DeathImage.PNG"), Properties.Resources.DeathImage);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "dfgdth.PNG"), Properties.Resources.dfgdth);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "dog-stare-zoom.png"), Properties.Resources.dog_stare_zoom);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "drgd.PNG"), Properties.Resources.drgd);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "drghdtfhftgiu.PNG"), Properties.Resources.drghdtfhftgiu);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "dug.png"), Properties.Resources.dug);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "dyftuftyuure.PNG"), Properties.Resources.dyftuftyuure);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "earinfection.png"), Properties.Resources.earinfection);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "F5GRMlEWUAAAiym.jpg"), Properties.Resources.F5GRMlEWUAAAiym);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "FbyFm47UcAEI1Cq.jpeg"), Properties.Resources.FbyFm47UcAEI1Cq);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "FhOuLBjWIAwab1v.png"), Properties.Resources.FhOuLBjWIAwab1v);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "Fuel.png"), Properties.Resources.Fuel);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "FwM5Y36XoAEO7r8.jpg"), Properties.Resources.FwM5Y36XoAEO7r8);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "Fx-3ZYsWcAIDlzG.jpg"), Properties.Resources.Fx_3ZYsWcAIDlzG);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "gobrae.PNG"), Properties.Resources.gobrae);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "gorp.png"), Properties.Resources.gorp);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "herobrine.png"), Properties.Resources.herobrine);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "jnjhk.PNG"), Properties.Resources.jnjhk);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "kettamean.jpg"), Properties.Resources.kettamean);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "maxresdefault (44).jpg"), Properties.Resources.maxresdefault__44_);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "ohdatsJudment.jpg"), Properties.Resources.ohdatsJudment);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "r.png"), Properties.Resources.r);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "sergsrhds.PNG"), Properties.Resources.sergsrhds);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "skol.PNG"), Properties.Resources.skol);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "SmartSelect_20211116-174331.jpg"), Properties.Resources.SmartSelect_20211116_174331);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "SmartSelect_20211125-164814.jpg"), Properties.Resources.SmartSelect_20211125_164814);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "SmartSelect_s20210321-152814.jpg"), Properties.Resources.SmartSelect_s20210321_152814);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "soypoint.png"), Properties.Resources.soypoint);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "stare.PNG"), Properties.Resources.stare);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "steamhappy.png"), Properties.Resources.steamhappy);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "Stinker.PNG"), Properties.Resources.Stinker);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "t56f7yit657yf7t6fyiuk.PNG"), Properties.Resources.t56f7yit657yf7t6fyiuk);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "tarwaee.PNG"), Properties.Resources.tarwaee);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "unnamed (4).jpg"), Properties.Resources.unnamed__4_);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "vgvhure.PNG"), Properties.Resources.vgvhure);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "yeahlmao.PNG"), Properties.Resources.yeahlmao);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "20230331_153754.jpg"), Properties.Resources._20230331_153754);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "20230420_163631.jpg"), Properties.Resources._20230420_163631);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "absoluteterror.png"), Properties.Resources.absoluteterror);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "aetsesye.PNG"), Properties.Resources.aetsesye);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "afarft.jpg"), Properties.Resources.afarft);
                ValidateBuiltInImage(Path.Combine(s_jumpscarePicsPath, "artworks-dgGS7o4DGuYHt1xX-IjeBqw-t500x500.jpg"), Properties.Resources.artworks_dgGS7o4DGuYHt1xX_IjeBqw_t500x500);
            }
        }
    }
}

