using System;
using System.Collections;

namespace ILLightenComparer.Emit.Reflection
{
    internal static class MethodName
    {
        public const string Factory = "CreateInstance";
        public const string Compare = nameof(IComparer.Compare);
        public const string CompareTo = nameof(IComparable.CompareTo);
    }
}
