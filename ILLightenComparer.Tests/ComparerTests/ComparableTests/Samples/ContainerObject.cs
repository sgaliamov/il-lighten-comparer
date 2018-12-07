using System.Collections.Generic;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public class ContainerObject
    {
        public static IComparer<ContainerObject> Comparer { get; } = new RelationalComparer();

        public ComparableObject ComparableProperty { get; set; }

        private sealed class RelationalComparer : IComparer<ContainerObject>
        {
            public int Compare(ContainerObject x, ContainerObject y)
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

                return Comparer<ComparableObject>.Default.Compare(x.ComparableProperty, y.ComparableProperty);
            }
        }
    }
}
