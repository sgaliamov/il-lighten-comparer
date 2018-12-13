using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples
{
    public class ArrayValue<T>
        where T : struct, IComparable<T>
    {
        public static IComparer<ArrayValue<T>> Comparer { get; } = new RelationalComparer();

        public T[] ArrayProperty { get; set; }

        private sealed class RelationalComparer : IComparer<ArrayValue<T>>
        {
            public int Compare(ArrayValue<T> x, ArrayValue<T> y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (ReferenceEquals(null, y))
                {
                    return 1;
                }

                if (ReferenceEquals(null, x))
                {
                    return -1;
                }

                return x.ArrayProperty.CompareTo(y.ArrayProperty);
            }
        }
    }
}
