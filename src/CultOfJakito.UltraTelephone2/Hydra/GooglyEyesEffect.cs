using Configgy;
using CultOfJakito.UltraTelephone2.Assets;
using CultOfJakito.UltraTelephone2.Chaos;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using HarmonyLib;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Hydra;

[RegisterChaosEffect]
[HarmonyPatch]
public class GooglyEyesEffect : ChaosEffect
{
    private static bool s_effectActive;
    private static UniRandom s_random;

    [Configgable("Hydra/Chaos", "Googly Eyes")]
    private static ConfigToggle s_enabled = new(true);

    public override void BeginEffect(UniRandom rand)
    {
        s_random = rand;
        s_effectActive = true;
    }

    public override int GetEffectCost() => 1;
    private void OnDestroy() => s_effectActive = false;


    [HarmonyPatch(typeof(EnemyIdentifier), "Start"), HarmonyPostfix]
    private static void OnEnemyStart(EnemyIdentifier __instance)
    {
        if (!s_effectActive || !s_enabled.Value)
            return;

        switch (__instance.enemyType)
        {
            case EnemyType.Filth:
                ApplyGooglyEyes_Filth(__instance);
                break;
            case EnemyType.Stray:
                ApplyGooglyEyes_Stray(__instance);
                break;
        }
    }


    private static void ApplyGooglyEyes_Filth(EnemyIdentifier __instance)
    {
        SeasonalHats hat = __instance.GetComponentInChildren<SeasonalHats>(true);
        if (hat == null)
            return;

        GooglyEye eye = GooglyEye.New();
        eye.transform.SetParent(hat.transform);
        eye.transform.localPosition = new Vector3(0.03283036f, 0.3157102f, 0.05048939f);
        eye.transform.localRotation = Quaternion.Euler(-52.167f, 15.641f, 63.549f);
        eye.transform.localScale = Vector3.one*0.1765201f;

        GooglyEye eye2 = GooglyEye.New();
        eye2.transform.SetParent(hat.transform);
        eye2.transform.localPosition = new Vector3(0.03624479f, 0.309489f, -0.06832973f);
        eye2.transform.localRotation = Quaternion.Euler(-52.263f, 146.45f, -53.117f);
        eye2.transform.localScale = Vector3.one * 0.1765201f;
    }

    private static void ApplyGooglyEyes_Stray(EnemyIdentifier __instance)
    {

    }
}

public class GooglyEye : MonoBehaviour
{
    public static GooglyEye New()
    {
        GameObject newGooglyEye = GameObject.Instantiate(HydraAssets.GooglyEye);
        return newGooglyEye.AddComponent<GooglyEye>();
    }

    private Transform center;
    private Transform eyeTransform;
    private Transform radiusMarkTransform;
    private float energy;
    private float energyDecayRate = 5f;

    float maxRadius;

    private void Awake()
    {
        center = transform.Find("EyeRoot");
        eyeTransform = transform.Find("EyeRoot/Eye");
        radiusMarkTransform = transform.Find("LowPosition");
    }

    private void Start()
    {
        maxRadius = Vector3.Distance(center.position, radiusMarkTransform.position);
        positionLastFrame = transform.position;
    }

    private void LateUpdate()
    {
        energy = Mathf.Max(0, energy - Time.deltaTime * energyDecayRate);
        Vector3 centerPoint = center.position;
        float sin = Mathf.Sin(Time.time*energy);

        if(energy == 0f)
            sin = 0f;

        float angle = sin * Mathf.Min(energy, 10f)*10f;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        rot = transform.rotation * rot;
        Vector3 offset = rot * Vector3.down*maxRadius;
        eyeTransform.position = centerPoint + offset;

        energy = Mathf.Min(energy + (transform.position - positionLastFrame).magnitude/2f, 20f);
        positionLastFrame = transform.position;
    }

    private Vector3 positionLastFrame;
    
}
