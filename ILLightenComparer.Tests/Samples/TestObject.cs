using System.Collections.Generic;

namespace ILLightenComparer.Tests.Samples
{
    public class TestObject
    {
        public double DoubleProperty { get; set; }
        public int IntegerProperty { get; set; }

        public static IComparer<TestObject> Comparer { get; } =
            new RelationalComparer();

        public override string ToString() =>
            $"{nameof(DoubleProperty)} = {DoubleProperty}, {nameof(IntegerProperty)} = {IntegerProperty}";

        private sealed class RelationalComparer : IComparer<TestObject>
        {
            public int Compare(TestObject x, TestObject y)
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

                var doublePropertyComparison = x.DoubleProperty.CompareTo(y.DoubleProperty);
                if (doublePropertyComparison != 0)
                {
                    return doublePropertyComparison;
                }

                return x.IntegerProperty.CompareTo(y.IntegerProperty);
            }
        }
    }
}
