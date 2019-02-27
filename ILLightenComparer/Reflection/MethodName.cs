using System;
using System.Collections;

namespace ILLightenComparer.Reflection
{
    internal static class MethodName
    {
        public const string CreateInstance = nameof(CreateInstance);

        public const string HasValue = nameof(Nullable<byte>.HasValue);
        public const string Value = nameof(Nullable<byte>.Value);

        public const string Get = nameof(Get);
        public const string Length = nameof(Array.Length);

        public const string Compare = nameof(IComparer.Compare);
        public const string CompareTo = nameof(IComparable.CompareTo);

        public const string GetEnumerator = nameof(IEnumerable.GetEnumerator);
        public const string Current = nameof(IEnumerator.Current);
    }
}
