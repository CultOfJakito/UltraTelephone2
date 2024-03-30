using UnityEngine;

namespace CultOfJakito.UltraTelephone2.UI
{
    public class ShakeyUI : MonoBehaviour
    {
        public float maxShakeRange = 0.5f;

        private float shakeTime = 0f;
        private float shakeAmount = 0f;

        private float lastShakeTime = 0f;

        private Vector3 originalPos;

        private bool manualShake = false;

        protected void Awake()
        {
            originalPos = transform.position;
        }

        public void Shake(float time, float amount)
        {
            shakeTime = Mathf.Max(0, time);
            lastShakeTime = Mathf.Max(0, time);
            shakeAmount = Mathf.Max(0, amount);
        }

        public void SetShaking(bool shaking, float amount)
        {
            this.manualShake = shaking;
            this.shakeAmount = Mathf.Clamp01(amount);
        }

        private void Update()
        {
            if (shakeTime <= 0f && !manualShake)
                return;

            float shake = (shakeTime/lastShakeTime);

            if (manualShake)
                shake = shakeAmount;
            else
                shake *= shakeAmount;

            Vector3 offset = UnityEngine.Random.insideUnitSphere * shake * maxShakeRange;
            offset.z = 0f;
            transform.position = originalPos + offset;

            if(!manualShake)
                shakeTime = Mathf.Max(0, shakeTime-Time.deltaTime);

            if(shakeTime <= 0f)
            {
                transform.position = originalPos;
            }
        }
    }
}
