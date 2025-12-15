using System;
using System.Collections.Generic;
using System.Text;

namespace Aeroverra.StreamDeck.Client.Actions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class InjectAttribute : Attribute
    {
    }
}
