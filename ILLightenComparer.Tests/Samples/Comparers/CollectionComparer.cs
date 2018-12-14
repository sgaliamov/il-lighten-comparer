using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples.Comparers
{
    internal sealed class CollectionComparer<TCollection, TItem> : IComparer<TCollection>
        where TCollection : IEnumerable<TItem>
    {
        private readonly IComparer<TItem> _itemComparer;

        public CollectionComparer(IComparer<TItem> itemComparer)
        {
            _itemComparer = itemComparer ?? Comparer<TItem>.Default;
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

                    var compare = _itemComparer.Compare(xCurrent, yCurrent);
                    if (compare != 0)
                    {
                        return compare;
                    }
                }
            }
        }
    }
}
