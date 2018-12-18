using System;
using System.Collections;

namespace ILLightenComparer.Emit.Reflection
{
    internal static class MethodName
    {
        public const string Factory = "CreateInstance";
        public const string HasValue = nameof(Nullable<byte>.HasValue);
        public const string Value = nameof(Nullable<byte>.Value);
        public const string ArrayGet = "Get";
        public const string ArrayLength = nameof(Array.Length);
        public const string Compare = nameof(IComparer.Compare);
        public const string CompareTo = nameof(IComparable.CompareTo);
        public const string GetEnumerator = nameof(IEnumerable.GetEnumerator);
    }
}
