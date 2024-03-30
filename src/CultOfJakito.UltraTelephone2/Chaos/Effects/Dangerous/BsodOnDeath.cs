using System.Runtime.InteropServices;
using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;
using CultOfJakito.UltraTelephone2.Events;
using HarmonyLib;
using static CultOfJakito.UltraTelephone2.GeneralSettings;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects.Dangerous
{
    [RegisterChaosEffect]
    [HarmonyPatch]
    internal class BsodOnDeath : ChaosEffect
    {
        [Configgable("Chaos/Effects/Dangerous", "BSOD On Death")]
        private static ConfigToggle s_enabled = new ConfigToggle(false);

        private static uint[] s_errors =
        {
            0xAD105, //adios
            0xDECEA5ED, //decreased
            0xBA5ED, //based
            0xF0CCAC1A, //foccacia
        };
        private bool s_effectActive = false;
        private UniRandom _rng;

        [DllImport("ntdll.dll")]
        private static extern uint RtlAdjustPrivilege(
            int privilege,
            bool bEnablePrivilege,
            bool isThreadPrivilege,
            out bool previousValue
        );

        [DllImport("ntdll.dll")]
        private static extern uint NtRaiseHardError(
            uint errorStatus,
            uint numberOfParameters,
            uint unicodeStringParameterMask,
            IntPtr parameters,
            uint validResponseOption,
            out uint response
        );

        public override void BeginEffect(UniRandom random)
        {
            if (!DangerousEffectsEnabled.Value)
                return;

            s_effectActive = true;
            _rng = random;
            GameEvents.OnPlayerDeath += BSOD;
        }

        public override int GetEffectCost() => 15;

        public override bool CanBeginEffect(ChaosSessionContext ctx) => s_enabled.Value && DangerousEffectsEnabled.Value && base.CanBeginEffect(ctx);

        private void BSOD()
        {
            // second check because im nervous :pleading:
            if (!DangerousEffectsEnabled.Value)
                return;

            if (!s_enabled.Value || !s_effectActive)
                return;

            RtlAdjustPrivilege(19, true, false, out _);
            NtRaiseHardError(_rng.SelectRandom(s_errors), 0, 0, IntPtr.Zero, 6, out uint _);
        }

        protected override void OnDestroy()
        {
            s_effectActive = false;
            GameEvents.OnPlayerDeath -= BSOD;
        }

    }
}
