using Configgy;
using CultOfJakito.UltraTelephone2.Util;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.LevelInjection.Level_4_2
{
    [RegisterLevelInjector]
    public class DontLookAtTheSun : MonoBehaviour, ILevelInjector
    {
        [Configgable("Patches/Level", "Dont look at the sun")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private Camera cam;
        private Transform camera;
        private List<Transform> suns;
        private Image blindnessImage;
        private float blindnessTimer;
        private bool staringAtSun;

        private float maxBlindnessTime = 17.5f;

        string levelName;

        public void OnLevelLoaded(string levelName)
        {
            if(!CheckLevel(levelName) || !s_enabled.Value)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }

            this.levelName = levelName;
        }

        private bool CheckLevel(string levelName)
        {
            if (levelName == "Level 4-2")
                return true;

            if (levelName == "Level 4-4")
                return true;


            return false;
        }

        private void Start()
        {
            camera = CameraController.Instance.transform;
            cam = CameraController.Instance.cam;

            GameObject div = new GameObject("Blindness");
            div.layer = 5;

            div.transform.SetParent(CanvasController.Instance.GetComponent<RectTransform>());
            div.transform.localScale = Vector3.one;

            RectTransform rect = div.AddComponent<RectTransform>();
            rect.SetSiblingIndex(7);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(0, 0);

            castMask = LayerMask.GetMask("Outdoors", "Environment");
            blindnessImage = div.AddComponent<Image>();
            blindnessImage.raycastTarget = false;
            blindnessImage.color = new Color(0, 0, 0, 0);

            switch (levelName)
            {
                case "Level 4-2":
                    HandleLevel4_2();
                    break;
                case "Level 4-4":
                    HandleLevel4_4();
                    break;
            }
        }


        private void HandleLevel4_2()
        {
            TimeOfDayChanger timeOfDayChanger = Locator.LocateComponent<TimeOfDayChanger>("NightActivator");

            if (timeOfDayChanger == null)
                return;

            suns = new List<Transform>();
            suns.Add(timeOfDayChanger.sunSprite.transform);
        }

        const string fourFourSunOnePath = "Exterior/Untilted/Sun";
        const string fourFourSunTwoPath = "8 - Outro/8 Nonstuff/Untilted (Outro)/Sun";

        private void HandleLevel4_4()
        {
            suns = new List<Transform>();

            SpriteRenderer sunOne = Locator.LocateComponent<SpriteRenderer>(fourFourSunOnePath);
            if (sunOne == null)
                Debug.LogError("Sun not found.");
            else
                suns.Add(sunOne.transform);

            SpriteRenderer sunTwo = Locator.LocateComponent<SpriteRenderer>(fourFourSunTwoPath);
            if (sunTwo == null)
                Debug.LogError("Second sun not found.");
            else
                suns.Add(sunTwo.transform);
        }

        LayerMask castMask;
        bool sentMessage;

        private void FixedUpdate()
        {
            if (suns == null || cam == null || camera == null)
                return;

            staringAtSun = false;

            for (int i = 0; i < suns.Count; i++)
            {
                if (suns[i] == null)
                    continue;

                TickSun(suns[i]);
            }

            if (!staringAtSun)
                return;

            blindnessTimer += Time.fixedDeltaTime;

            float blindness = Mathf.Clamp01(blindnessTimer / maxBlindnessTime);
            blindnessImage.color = new Color(0f, 0f, 0f, blindness);

            if (!sentMessage && (blindnessTimer / maxBlindnessTime) > 0.8f)
            {
                sentMessage = true;
                HudMessageReceiver.Instance.SendHudMessage("Maybe I shouldn't stare at the sun...");
            }
        }

        private void TickSun(Transform sun)
        {
            if (sun == null || !sun.gameObject.activeInHierarchy)
                return;

            Vector3 sunPos = sun.position;
            Vector3 camPos = camera.position;

            Vector3 toSun = camPos - sunPos;
            Vector3 lookDir = camera.forward;

            float angle = Vector3.Angle(-toSun.normalized, lookDir.normalized);

            //not facing sun
            if (angle > cam.fieldOfView * 0.35f)
                return;

            float sunScaleY = sun.localScale.y;
            float castOffsetRadius = sunScaleY * 0.35f;

            Vector3 nextCastPos = sunPos + (Vector3.up * castOffsetRadius);
            Vector3 toPlayer = camPos - nextCastPos;
            bool topCast = Physics.Raycast(nextCastPos, toPlayer, toPlayer.magnitude, castMask);

            nextCastPos = sunPos + (sun.right * castOffsetRadius);
            toPlayer = camPos - nextCastPos;
            bool leftCast = Physics.Raycast(nextCastPos, toPlayer, toPlayer.magnitude, castMask);

            nextCastPos = sunPos + (-sun.right * castOffsetRadius);
            toPlayer = camPos - nextCastPos;
            bool rightCast = Physics.Raycast(nextCastPos, toPlayer, toPlayer.magnitude, castMask);

            staringAtSun |= !topCast || !leftCast || !rightCast;
        }
    }


}
