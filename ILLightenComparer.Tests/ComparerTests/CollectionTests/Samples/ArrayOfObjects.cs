using System.Collections.Generic;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples
{
    public class ArrayOfObjects<T>
    {
        public static IComparer<ArrayOfObjects<T>> Comparer { get; } = new RelationalComparer();

        public T[] ArrayProperty { get; set; }

        private sealed class RelationalComparer : IComparer<ArrayOfObjects<T>>
        {
            public int Compare(ArrayOfObjects<T> x, ArrayOfObjects<T> y)
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
