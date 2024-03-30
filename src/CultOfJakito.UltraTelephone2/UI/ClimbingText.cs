using TMPro;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.UI
{
    public class ClimbingText : MonoBehaviour
    {
        private TextMeshProUGUI textObj;
        public TextMeshProUGUI TextObj
        {
            get
            {
                if (textObj == null)
                    textObj = GetComponent<TextMeshProUGUI>();
                return textObj;
            }
        }

        public float climbSpeed = 0.35f;

        public AudioSource tickSource;

        private float lastSetTime = 0f;
        private long lastCurrentValue;

        private long targetValue;
        private long currentValue;

        public Func<long, string> toString;

        private void Update()
        {
            if (currentValue != targetValue)
            {
                long lastCur = currentValue;
                float lastCurVal = lastCurrentValue;
                float tarVal = targetValue;

                float diff = Time.time-lastSetTime;
                float t = Mathf.Clamp01(diff/climbSpeed);
                t = Mathf.SmoothStep(0, 1, t);

                float curVal = Mathf.Lerp(lastCurVal, tarVal, t);
                currentValue = Mathf.CeilToInt(curVal);

                if(currentValue-lastCur > 0)
                {
                    if(tickSource)
                        tickSource.Play();
                }

                string text = (toString == null) ? currentValue.ToString("000") : toString.Invoke(currentValue);
                TextObj.text = currentValue.ToString("000");
            }
        }

        public void SetTargetValue(long value)
        {
            this.targetValue = value;
            lastCurrentValue = currentValue;
            lastSetTime = Time.time;
        }

        public void SetValue(long value)
        {
            lastSetTime = Time.time;
            lastCurrentValue = currentValue;
            this.targetValue = value;
            this.currentValue = value;
            TextObj.text = (toString == null) ? currentValue.ToString("000") : toString.Invoke(currentValue);
        }

        private void OnEnable()
        {
            lastSetTime = Time.time;
        }
    }
}
