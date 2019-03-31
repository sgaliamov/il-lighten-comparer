using System.Collections.Generic;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests.CycleTests.Samples
{
    public sealed class SelfSealed
    {
        public readonly int Id;
        public SelfSealed First;

        public SelfSealed()
        {
            Id = this.GetObjectId();
        }

        public static RelationalComparer Comparer { get; } = new RelationalComparer();
        public SelfSealed Second { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }

        public sealed class RelationalComparer : IComparer<SelfSealed>
        {
            public int Compare(SelfSealed x, SelfSealed y)
            {
                var setX = new ConcurrentSet<object>();
                var setY = new ConcurrentSet<object>();

                return Compare(x, y, setX, setY);
            }

            private static int Compare(SelfSealed x, SelfSealed y, ConcurrentSet<object> setX, ConcurrentSet<object> setY)
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

                if (!setX.TryAdd(x, 0) & !setY.TryAdd(y, 0))
                {
                    return setX.Count - setY.Count;
                }

                var compareFirst = Compare(x.First, y.First, setX, setY);
                if (compareFirst != 0)
                {
                    return compareFirst;
                }

                var compareSecond = Compare(x.Second, y.Second, setX, setY);
                if (compareSecond != 0)
                {
                    return compareSecond;
                }

                var compareValue = x.Value.CompareTo(y.Value);
                if (compareValue != 0)
                {
                    return compareValue;
                }

                return 0;
            }
        }
    }
}
