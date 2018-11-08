using System;
using System.Collections;
using System.Collections.Generic;

namespace ILightenComparer.Reflection
{
    public static class Interface
    {
        public static readonly Type Comparer = typeof(IComparer);
        public static readonly Type GenericComparer = typeof(IComparer<>);
    }
}
