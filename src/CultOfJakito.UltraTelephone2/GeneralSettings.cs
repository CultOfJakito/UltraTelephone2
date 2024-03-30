using Configgy;

namespace CultOfJakito.UltraTelephone2
{
    public static class GeneralSettings
    {
        [Configgable("General", description:"If copyrighted music should be enabled or disabled.")]
        public static ConfigToggle EnableCopyrightedMusic = new ConfigToggle(true);

        [Configgable("General", displayName:"Personalization", description:"(Restart Required) How personalized of an experience do you want?")]
        public static ConfigDropdown<PersonalizationLevel> Personalization = new ConfigDropdown<PersonalizationLevel>((PersonalizationLevel[])Enum.GetValues(typeof(PersonalizationLevel)), defaultIndex:(int)PersonalizationLevel.NotMuch);

        [Configgable("Extras", displayName:"Dangerous Effects", description:"Ill advised to enable. Enables dangerous, harmful, or horny chaos effects. These may cause permenant damage to your computer or phsyce")]
        public static ConfigToggle DangerousEffectsEnabled = new ConfigToggle(false);
    }
}
