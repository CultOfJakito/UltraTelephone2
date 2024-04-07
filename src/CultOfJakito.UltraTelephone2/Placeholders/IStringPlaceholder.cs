using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Placeholders
{
    public interface IStringPlaceholder
    {
        public string PlaceholderID { get; }
        public string GetPlaceholderValue();
    }
}
