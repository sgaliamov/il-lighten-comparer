using System;
using System.Collections.Generic;
using System.Linq;

namespace ILLightenComparer.Tests.Samples.Comparers
{
    internal sealed class CollectionComparer<TCollection, TItem> : IComparer<TCollection>
        where TCollection : IEnumerable<TItem>
    {
        private readonly bool _sort;
        private readonly IComparer<TItem> _itemComparer;

        public CollectionComparer(IComparer<TItem> itemComparer, bool sort)
        {
            _sort = sort;
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

            if (_sort)
            {
                var ax = x.ToArray();
                Array.Sort(ax, _itemComparer);
                x = (TCollection)ax.AsEnumerable();

                var ay = y.ToArray();
                Array.Sort(ay, _itemComparer);
                y = (TCollection)ay.AsEnumerable();
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
