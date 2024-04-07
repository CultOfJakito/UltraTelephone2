using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Placeholders
{
    public static class PlaceholderHelper
    {
        public const string PlaceholderStart = "<ph>";
        public const string PlaceholderEnd = "</ph>";
        public static bool ContainsPlaceholder(string value)
        {
            return value.Contains(PlaceholderStart) && value.Contains(PlaceholderEnd);
        }

        public static string ReplacePlaceholders(string value)
        {
            if(!ContainsPlaceholder(value))
                return value;

            PlaceholderDatabase mainDatabase = UltraTelephoneTwo.Instance.PlaceholderDatabase;
            return mainDatabase.ReplacePlaceholders(value);
        }
    }
}
