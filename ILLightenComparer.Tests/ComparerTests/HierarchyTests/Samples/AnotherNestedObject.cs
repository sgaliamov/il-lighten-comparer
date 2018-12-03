using System;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples
{
    public sealed class AnotherNestedObject : INestedObject
    {
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is AnotherNestedObject other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(AnotherNestedObject)}.");
        }

        public string Text { get; set; }

        private int CompareTo(INestedObject other) => string.Compare(Text, other.Text, StringComparison.Ordinal);
    }
}
