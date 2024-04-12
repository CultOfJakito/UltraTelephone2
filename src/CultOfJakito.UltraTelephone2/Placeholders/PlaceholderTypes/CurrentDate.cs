namespace CultOfJakito.UltraTelephone2.Placeholders.PlaceholderTypes
{
    public class CurrentDate : IStringPlaceholder
    {
        public string PlaceholderID => "CURRENT_DATE";

        public string GetPlaceholderValue()
        {
            return DateTime.Now.ToString("dd/MM");
        }
    }
}
