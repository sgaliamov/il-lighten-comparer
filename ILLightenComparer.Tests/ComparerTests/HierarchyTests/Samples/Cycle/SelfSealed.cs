using System.Collections.Generic;
using System.Threading;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycle
{
    public sealed class SelfSealed
    {
        private static int _counter;

        public SelfSealed Self;

        public SelfSealed() => Id = Interlocked.Increment(ref _counter);

        public static RelationalComparer Comparer { get; } = new RelationalComparer();

        public int Id { get; }

        public override string ToString() => Id.ToString();

        public sealed class RelationalComparer : IComparer<SelfSealed>
        {
            public int Compare(SelfSealed x, SelfSealed y)
            {
                var setX = new HashSet<object> { x };
                var setY = new HashSet<object> { y };

                return Compare(setX, setY, x, y);
            }

            private static int Compare(ISet<object> setX, ISet<object> setY, SelfSealed x, SelfSealed y)
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

                if (!setX.Contains(x.Self) || !setY.Contains(y.Self))
                {
                    if (x.Self != null)
                    {
                        setX.Add(x.Self);
                    }

                    if (y.Self != null)
                    {
                        setY.Add(y.Self);
                    }

                    var compare = Compare(setX, setY, x.Self, y.Self);
                    
                    setX.Remove(x.Self);
                    setY.Remove(y.Self);

                    if (compare != 0)
                    {
                        return compare;
                    }
                }

                return x.Id.CompareTo(y.Id);
            }
        }
    }
}
