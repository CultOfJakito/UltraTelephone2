using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.DependencyInjection;

[System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
sealed class InjectAttribute : Attribute {
}
