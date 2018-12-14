using System.Collections.Generic;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests.Samples
{
    public class ArrayOfObjects<T>
    {
        public T[] ArrayProperty { get; set; }

        public static IComparer<ArrayOfObjects<T>> GetComparer(IComparer<T> comparer)
        {
            return new RelationalComparer(comparer);
        }

        public override string ToString()
        {
            return string.Join(", ", ArrayProperty);
        }

        private sealed class RelationalComparer : IComparer<ArrayOfObjects<T>>
        {
            private readonly CollectionComparer<IEnumerable<T>, T> _collectionComparer;

            public RelationalComparer(IComparer<T> comparer)
            {
                _collectionComparer = new CollectionComparer<IEnumerable<T>, T>(comparer);
            }

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

                return _collectionComparer.Compare(x.ArrayProperty, y.ArrayProperty);
            }
        }
    }
}
