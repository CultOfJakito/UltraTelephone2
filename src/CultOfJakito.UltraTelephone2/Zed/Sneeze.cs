using System.Collections;
using Configgy;
using CultOfJakito.UltraTelephone2.Chaos;
using HarmonyLib;
using UnityEngine;

//[RegisterChaosEffect]
public class Sneeze : ChaosEffect
{
    //[Configgable("Chaos/Effects", "Sneeze")]
    private static ConfigToggle s_enabled = new(true);

    public override void BeginEffect(UniRandom random)
    {

    }

    public override bool CanBeginEffect(ChaosSessionContext ctx)
    {
        if (!s_enabled.Value)
            return false;

        if(sneezeEvent is null)
        {
            sneezeEvent = new UltrakillEvent();
            sneezeEvent.onActivate.AddListener(EndSneeze);
        }

        return base.CanBeginEffect(ctx);
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
