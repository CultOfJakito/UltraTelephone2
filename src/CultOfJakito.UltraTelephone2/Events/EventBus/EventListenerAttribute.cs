using System;
using System.Collections.Generic;
using System.Text;

namespace CultOfJakito.UltraTelephone2.Events
{
    /// <summary>
    /// Marks a method as an event listener.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EventListenerAttribute : Attribute
    {
    }
}
