using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;

namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples
{
    public class AbstractProperties
    {
        public static IComparer<AbstractProperties> Comparer { get; } = new AbstractPropertiesRelationalComparer();
        public AbstractNestedObject AbstractProperty { get; set; }
        public INestedObject InterfaceProperty { get; set; }
        public BaseNestedObject NotSealedProperty { get; set; }
        public object ObjectProperty { get; set; }

        private sealed class AbstractPropertiesRelationalComparer : IComparer<AbstractProperties>
        {
            public int Compare(AbstractProperties x, AbstractProperties y)
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

                var abstractPropertyComparison = Comparer<AbstractNestedObject>.Default.Compare(x.AbstractProperty, y.AbstractProperty);
                if (abstractPropertyComparison != 0)
                {
                    return abstractPropertyComparison;
                }

                var interfacePropertyComparison = Comparer<INestedObject>.Default.Compare(x.InterfaceProperty, y.InterfaceProperty);
                if (interfacePropertyComparison != 0)
                {
                    return interfacePropertyComparison;
                }

                var compare = Comparer<BaseNestedObject>.Default.Compare(x.NotSealedProperty, y.NotSealedProperty);
                if (compare != 0)
                {
                    return compare;
                }

                if (x.ObjectProperty == null)
                {
                    if (y.ObjectProperty == null)
                    {
                        return 0;
                    }

                    return -1;
                }

                if (y.ObjectProperty == null)
                {
                    return 1;
                }

                if (x.ObjectProperty is IComparable comparable
                    && y.ObjectProperty.GetType() == x.ObjectProperty.GetType())
                {
                    return comparable.CompareTo(y.ObjectProperty);
                }

                throw new InvalidOperationException("ObjectProperty is not comparable.");
            }
        }
    }
}
