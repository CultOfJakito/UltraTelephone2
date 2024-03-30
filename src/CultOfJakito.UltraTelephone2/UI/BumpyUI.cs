using UnityEngine;

namespace CultOfJakito.UltraTelephone2.UI
{
    public class BumpyUI : MonoBehaviour
    {
        public float maxBumpScale = 1.4f;

        private float bumpTime = 0f;
        private float lastBumpTime = 0f;

        private Vector3 originalScale;

        protected void Awake()
        {
            originalScale = transform.localScale;
        }

        public void Bump(float length)
        {
            this.bumpTime = Mathf.Max(0, length);
            this.lastBumpTime = bumpTime;
        }

        private void Update()
        {
            if (bumpTime <= 0f)
                return;

            bumpTime = Mathf.Max(0, bumpTime-Time.deltaTime);
            float shake = 1f - (bumpTime/lastBumpTime);

            Vector3 scale = originalScale * (1f + (1-shake * maxBumpScale));
            transform.localScale = scale;

            if (bumpTime <= 0f)
            {
                transform.localScale = originalScale;
            }
        }
    }
}
