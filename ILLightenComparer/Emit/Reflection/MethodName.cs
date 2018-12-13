using System;
using System.Collections;

namespace ILLightenComparer.Emit.Reflection
{
    internal static class MethodName
    {
        public const string Factory = "CreateInstance";
        public const string HasValue = "HasValue";
        public const string Value = "Value";
        public const string GetItem = "get_Item";
        public const string Count = nameof(ICollection.Count);
        public const string Compare = nameof(IComparer.Compare);
        public const string CompareTo = nameof(IComparable.CompareTo);
    }
}
