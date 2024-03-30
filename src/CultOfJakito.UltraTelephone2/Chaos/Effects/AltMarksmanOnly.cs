using Configgy;
using CultOfJakito.UltraTelephone2.DependencyInjection;

namespace CultOfJakito.UltraTelephone2.Chaos.Effects
{
    [RegisterChaosEffect]
    public class AltMarksmanOnly : ChaosEffect
    {
        [Configgable("Chaos/Effects", "Alt Marksman Only")]
        private static ConfigToggle s_enabled = new ConfigToggle(true);

        public override void BeginEffect(UniRandom random)
        {
            ForceLoadout();
        }

        private void ForceLoadout()
        {
            ForcedLoadout loadout = new ForcedLoadout();

            VariantSetting varOff = new VariantSetting()
            {
                blueVariant = VariantOption.ForceOff,
                redVariant = VariantOption.ForceOff,
                greenVariant = VariantOption.ForceOff,
            };

            loadout.arm = new ArmVariantSetting()
            {
                blueVariant = VariantOption.IfEquipped,
                redVariant = VariantOption.IfEquipped,
                greenVariant = VariantOption.IfEquipped,
            };

            loadout.revolver = varOff;
            loadout.shotgun = varOff;
            loadout.altNailgun = varOff;
            loadout.nailgun = varOff;
            loadout.rocketLauncher = varOff;
            loadout.railcannon = varOff;

            loadout.altRevolver = new VariantSetting()
            {
                blueVariant = VariantOption.ForceOff,
                redVariant = VariantOption.ForceOff,
                greenVariant = VariantOption.ForceOn,
            };

            GunSetter.Instance.forcedLoadout = loadout;
            GunSetter.Instance.ResetWeapons(false);

            FistControl.Instance.forcedLoadout = loadout;
            FistControl.Instance.ResetFists();
        }

        private void ResetLoadout()
        {
            GunSetter.Instance.forcedLoadout = null;
            GunSetter.Instance.ResetWeapons(false);

            FistControl.Instance.forcedLoadout = null;
            FistControl.Instance.ResetFists();
        }

        public override void Dispose()
        {
            ResetLoadout();
            base.Dispose();
        }

        protected override void OnDestroy() { }

        public override bool CanBeginEffect(ChaosSessionContext ctx)
        {
            //Not in secret levels probably or intermissions
            if(ctx.LevelName.Contains("-S") || ctx.LevelName.Contains("Intermission"))
                return false;

            return s_enabled.Value && base.CanBeginEffect(ctx);
        }

        public override int GetEffectCost() => 10;
    }
}
