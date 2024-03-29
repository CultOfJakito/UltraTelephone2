using Configgy;
using UnityEngine;
using UnityEngine.UI;

namespace CultOfJakito.UltraTelephone2.LevelInjection.Level_4_2
{
    [RegisterLevelInjector]
    public class DontLookAtTheSun : MonoBehaviour, ILevelInjector
    {
        [Configgable("Patches/Level/4-2", "Dont look at the sun")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        private Camera cam;
        private Transform camera;
        private Transform sun;
        private Image blindnessImage;
        private float blindnessTimer;
        private bool staringAtSun;

        private float maxBlindnessTime = 17.5f;

        public void OnLevelLoaded(string levelName)
        {
            if(levelName != "Level 4-2" || !s_enabled.Value)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
        }

        private void Start()
        {
            camera = CameraController.Instance.transform;
            cam = CameraController.Instance.cam;
            TimeOfDayChanger timeOfDayChanger = RandomExtensions.FindObjectsOfTypeIncludingInactive<TimeOfDayChanger>().Where(x => x.name == "NightActivator").FirstOrDefault();

            if (timeOfDayChanger == null)
                return;

            sun = timeOfDayChanger.sunSprite.transform;

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
        }

        LayerMask castMask;

        private void Update()
        {
            staringAtSun = false;

            Vector3 sunPos = sun.position;
            Vector3 camPos = camera.position;

            Vector3 toSun = camPos - sunPos;
            Vector3 lookDir = camera.forward;

            float angle = Vector3.Angle(-toSun.normalized, lookDir.normalized);

            //not facing sun
            if (angle > cam.fieldOfView*0.35f)
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

            staringAtSun = !topCast || !leftCast || !rightCast;

            if (!staringAtSun)
                return;

            blindnessTimer += Time.deltaTime;
            float blindness = Mathf.Clamp01(blindnessTimer / maxBlindnessTime);
            blindnessImage.color = new Color(0f, 0f, 0f, blindness);
        }

    }


}
