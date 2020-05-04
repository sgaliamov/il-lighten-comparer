using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ILLightenComparer.Tests.EqualityComparers
{
    internal sealed class CollectionEqualityComparer<TItem> : IEqualityComparer<IEnumerable<TItem>>, IEqualityComparer, IHashSeedSetter
    {
        private readonly IEqualityComparer<TItem> _itemComparer;
        private readonly bool _sort;
        private readonly AsyncLocal<long> _seed = new AsyncLocal<long>();

        public CollectionEqualityComparer(IEqualityComparer<TItem> itemComparer = null, bool sort = false)
        {
            _sort = sort;
            _itemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
            _seed.Value = HashCodeCombiner.Seed;
        }

        public bool Equals(IEnumerable<TItem> x, IEnumerable<TItem> y)
        {
            if (x == null) {
                return y == null;
            }

            if (y == null) {
                return false;
            }

            if (_sort) {
                var ax = x.ToArray();
                Array.Sort(ax);
                x = ax;

                var ay = y.ToArray();
                Array.Sort(ay);
                y = ay;
            }

            using var enumeratorX = x.GetEnumerator();
            using var enumeratorY = y.GetEnumerator();

            while (true) {
                var xDone = !enumeratorX.MoveNext();
                var yDone = !enumeratorY.MoveNext();

                if (xDone) {
                    return yDone;
                }

                if (yDone) {
                    return false;
                }

                var xCurrent = enumeratorX.Current;
                var yCurrent = enumeratorY.Current;

                var compare = _itemComparer.Equals(xCurrent, yCurrent);
                if (!compare) {
                    return false;
                }
            }
        }

        bool IEqualityComparer.Equals(object x, object y) => Equals((IEnumerable<TItem>)x, (IEnumerable<TItem>)y);

        public int GetHashCode(IEnumerable<TItem> obj)
        {
            if (obj is null) {
                return 0;
            }

            if (_sort) {
                var array = obj.ToArray();
                Array.Sort(array);
                obj = array;
            }

            return HashCodeCombiner.Start(_seed.Value).Combine(_itemComparer, obj.ToArray());
        }

        public int GetHashCode(object obj) => GetHashCode((IEnumerable<TItem>)obj);

        public void SetHashSeed(long seed) => _seed.Value = seed;
    }
}
