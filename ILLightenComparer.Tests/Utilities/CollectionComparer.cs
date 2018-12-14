using System.Collections.Generic;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class CollectionComparer<TCollection, TItem> : IComparer<TCollection>
        where TCollection : IEnumerable<TItem>
    {
        private readonly IComparer<TItem> _comparer;

        public CollectionComparer(IComparer<TItem> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(TCollection x, TCollection y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }

                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            using (var enumeratorX = x.GetEnumerator())
            using (var enumeratorY = y.GetEnumerator())
            {
                while (true)
                {
                    var xDone = !enumeratorX.MoveNext();
                    var yDone = !enumeratorY.MoveNext();

                    if (xDone)
                    {
                        if (yDone)
                        {
                            return 0;
                        }

                        return -1;
                    }

                    if (yDone)
                    {
                        return 1;
                    }

                    var xCurrent = enumeratorX.Current;
                    var yCurrent = enumeratorY.Current;

                    var compare = _comparer.Compare(xCurrent, yCurrent);
                    if (compare != 0)
                    {
                        return compare;
                    }
                }
            }
        }
    }
}
