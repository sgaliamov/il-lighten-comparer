using System;
using System.Collections;
using System.Collections.Generic;

namespace ILLightenComparer.Reflection
{
    internal static class Interface
    {
        public static readonly Type Comparer = typeof(IComparer);
        public static readonly Type EqualityComparer = typeof(IEqualityComparer);
        public static readonly Type GenericComparer = typeof(IComparer<>);
        public static readonly Type GenericEqualityComparer = typeof(IEqualityComparer<>);
    }
}
