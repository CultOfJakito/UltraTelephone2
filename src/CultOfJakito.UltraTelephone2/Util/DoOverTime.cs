using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Util
{
    public static class DoOverTime
    {
        public static void DoAfterTime(this MonoBehaviour behaviour, float length, Action action)
        {
            behaviour.StartCoroutine(DoAfterTimeCoroutine(length, action));
        }

        public static void DoAfterTimeUnscaled(this MonoBehaviour behaviour, float length, Action action)
        {
            behaviour.StartCoroutine(DoAfterTimeUnscaledCoroutine(length, action));
        }

        public static void DoAfterTimeFixedUpdate(this MonoBehaviour behaviour, float length, Action action)
        {
            behaviour.StartCoroutine(DoAfterTimeFixedUpdateCoroutine(length, action));
        }

        private static IEnumerator DoAfterTimeCoroutine(float length, Action action)
        {
            float timer = length;
            while (timer > 0f)
            {
                yield return new WaitForEndOfFrame();
                timer = Mathf.Max(0f, timer - Time.deltaTime);
            }

            action?.Invoke();
        }

        private static IEnumerator DoAfterTimeUnscaledCoroutine(float length, Action action)
        {
            float timer = length;
            while (timer > 0f)
            {
                yield return new WaitForEndOfFrame();
                timer = Mathf.Max(0f, timer - Time.unscaledDeltaTime);
            }

            action?.Invoke();
        }

        private static IEnumerator DoAfterTimeFixedUpdateCoroutine(float length, Action action)
        {
            float timer = length;
            while (timer > 0f)
            {
                yield return new WaitForFixedUpdate();
                timer = Mathf.Max(0f, timer - Time.fixedDeltaTime);
            }

            action?.Invoke();
        }
    }
}
