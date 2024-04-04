using Configgy;

namespace CultOfJakito.UltraTelephone2
{
    public static class GeneralSettings
    {
        [Configgable(displayName:"Copyrighted Music", description:"If copyrighted music should be enabled or disabled.")]
        public static ConfigToggle EnableCopyrightedMusic = new ConfigToggle(true);

        [Configgable(displayName:"Personalization", description:"(Restart required) How personalized of an experience do you want?")]
        public static ConfigDropdown<PersonalizationLevel> Personalization = new ConfigDropdown<PersonalizationLevel>((PersonalizationLevel[])Enum.GetValues(typeof(PersonalizationLevel)), defaultIndex:(int)PersonalizationLevel.NotMuch);

        [Configgable("Extras", displayName:"Dangerous Effects", description:"Ill advised to enable. Enables dangerous, harmful, or horny chaos effects. These may cause permanent damage to your computer or psyche.")]
        public static ConfigToggle DangerousEffectsEnabled = new ConfigToggle(false);
    }
}
