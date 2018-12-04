using System.Collections.Generic;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Cycle
{
    public sealed class SelfSealed
    {
        public SelfSealed First;

        public static RelationalComparer Comparer { get; } = new RelationalComparer();

        public SelfSealed Second { get; set; }
        public int Value { get; set; }

        public override string ToString() => this.GetObjectId().ToString();

        public sealed class RelationalComparer : IComparer<SelfSealed>
        {
            public int Compare(SelfSealed x, SelfSealed y)
            {
                var setX = new HashSet<object> { x };
                var setY = new HashSet<object> { y };

                return Compare(x, y, setX, setY);
            }

            private static int Compare(SelfSealed x, SelfSealed y, ISet<object> xSet, ISet<object> ySet)
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

                var compareFirst = TryCompare(x.First, y.First, xSet, ySet);
                if (compareFirst != 0)
                {
                    return compareFirst;
                }

                var compareSecond = TryCompare(x.Second, y.Second, xSet, ySet);
                if (compareSecond != 0)
                {
                    return compareSecond;
                }

                var compareValue = x.Value.CompareTo(y.Value);
                if (compareValue != 0)
                {
                    return compareValue;
                }

                return xSet.Count - ySet.Count;
            }

            private static int TryCompare(
                SelfSealed x,
                SelfSealed y,
                ISet<object> xSet,
                ISet<object> ySet)
            {
                if (xSet.Contains(x) && ySet.Contains(y))
                {
                    return 0;
                }

                xSet.Add(x);
                ySet.Add(y);

                var compare = Compare(x, y, xSet, ySet);

                xSet.Remove(x);
                ySet.Remove(y);

                return compare;

            }
        }
    }
}
