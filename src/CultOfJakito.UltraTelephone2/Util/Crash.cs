using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using Configgy;

namespace CultOfJakito.UltraTelephone2.Util
{
    public static class Crash
    {
        public static bool IsDestabilized { get; private set; }

        private static CoroutineRunner s_runner;
        private static CoroutineRunner GetRunner()
        {
            if(s_runner == null)
            {
                s_runner = new GameObject("UT2 Crash helper").AddComponent<CoroutineRunner>();
                GameObject.DontDestroyOnLoad(s_runner.gameObject);
            }
            return s_runner;
        }

        public static void RestoreStability()
        {
            if (!IsDestabilized)
                return;

            IsDestabilized = false;
            GetRunner().StopAllCoroutines();
        }

        public static void DestabilizingCrash()
        {
            if (IsDestabilized)
                return;

            IsDestabilized = true;
            GetRunner().StartCoroutine(GradualCrashRoutine(GetRunner()));
        }

        private static IEnumerator GradualCrashRoutine(MonoBehaviour behaviour)
        {
            while(true)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                behaviour.StartCoroutine(GradualCrashRoutine(behaviour));
            }
        }

        public static void Freeze()
        {
            while (true) { }
        }

        /// <summary>
        /// Causes the game to lag for a specified amount of time at a target FPS with a specified aggression
        /// </summary>
        /// <param name="length">Amount of time to lag</param>
        /// <param name="targetFPS">The fps were trying to reach</param>
        /// <param name="aggression">Rate of change to add or remove calls from the lag generation to reach the target FPS</param>
        public static void Lag(float length, float targetFPS, int aggression = 1)
        {
            GetRunner().StartCoroutine(LagGeneratorCoroutine(length, targetFPS, aggression));
        }

        private static IEnumerator LagGeneratorCoroutine(float length, float targetFPS, int aggression = 1)
        {
            int maxFPSTracks = 200;
            List<float> fpsList = new List<float>();

            float averageFPS = 1/Time.unscaledDeltaTime;
            int callAmount = 1;

            float timer = length;
            while(timer > 0)
            {
                float fps = 1/Time.unscaledDeltaTime;
                fpsList.Add(fps);

                if(fpsList.Count >= maxFPSTracks)
                    fpsList.RemoveAt(0);

                averageFPS = fpsList.Average();

                float distanceToTarget = targetFPS - averageFPS;

                //Adjust call amount based on distance to target
                if(distanceToTarget > 0)
                    callAmount -= aggression;
                else
                    callAmount += aggression;

                //Lag generator
                for (int i = 0; i < callAmount; i++)
                {
                    string cringe = "cringe";

                    foreach (Transform tf in GameObject.FindObjectsOfType<Transform>())
                    {
                        cringe += tf.name;
                    }

                    if(i %2==0)
                        cringe = "cringe";
                }

                yield return new WaitForEndOfFrame();
                timer -= Time.unscaledDeltaTime;
            }
        }

        public static void InstantCrash()
        {
            Application.Quit();
        }

        public static void CrashWithMessage(string title, string message, string button)
        {
            ModalDialogue.ShowDialogue(new ModalDialogueEvent()
            {
                Message = message,
                Title = title,
                Options = new DialogueBoxOption[]
                {
                    new DialogueBoxOption()
                    {
                        Name = button,
                        Color = Color.red,
                        OnClick = () => InstantCrash()
                    }
                }
            });
        }

    }
}
