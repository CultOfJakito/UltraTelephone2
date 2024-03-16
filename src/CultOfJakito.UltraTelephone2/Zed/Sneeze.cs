using CultOfJakito.UltraTelephone2.Chaos;
using UnityEngine;
using System.Collections;
using HarmonyLib;
using UnityEngine.Events;

public class Sneeze : ChaosEffect
{
    public override void BeginEffect(System.Random random)
    {

    }
    public override bool CanBeginEffect(ChaosSessionContext ctx)
    {
        if(sneezeEvent is null)
        {
            sneezeEvent = new UltrakillEvent();
            sneezeEvent.onActivate.AddListener(EndSneeze);
        }
        return true;
    }

    IEnumerator Countdown(float time)
    {
        yield return new WaitForSeconds(time);
        DoSneeze();
    }
    AudioClip cartoonSneeze = null;
    UltrakillEvent sneezeEvent = null;
    private void DoSneeze()
    {
        GameObject sneeze = new("Sneeze");
        sneeze.AddComponent<AudioSource>().clip = cartoonSneeze;
        sneeze.GetComponent<AudioSource>().Play();
        sneeze.AddComponent<ActivateOnSoundEnd>();
        Traverse.Create(sneeze.GetComponent<ActivateOnSoundEnd>()).Field("events").SetValue(sneezeEvent);
    }
    private void EndSneeze()
    {
        // Thanos snap the map
    }

    public override int GetEffectCost() => 1;
}