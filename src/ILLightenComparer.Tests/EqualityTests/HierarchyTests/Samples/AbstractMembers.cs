using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.EqualityTests.HierarchyTests.Samples.Nested;

namespace ILLightenComparer.Tests.EqualityTests.HierarchyTests.Samples
{
    public class AbstractMembers
    {
        public INestedObject InterfaceField;
        public object ObjectField;

        public AbstractNestedObject AbstractProperty { get; set; }
        public BaseNestedObject NotSealedProperty { get; set; }

        public override bool Equals(object obj) => Equals((AbstractMembers)obj);

        public bool Equals(AbstractMembers other)
        {
            return other != null &&
                   EqualityComparer<INestedObject>.Default.Equals(InterfaceField, other.InterfaceField) &&
                   EqualityComparer<object>.Default.Equals(ObjectField, other.ObjectField) &&
                   EqualityComparer<AbstractNestedObject>.Default.Equals(AbstractProperty, other.AbstractProperty) &&
                   EqualityComparer<BaseNestedObject>.Default.Equals(NotSealedProperty, other.NotSealedProperty);
        }

        public override int GetHashCode() => HashCodeCombiner.Combine(InterfaceField, ObjectField, AbstractProperty, NotSealedProperty);
    }
}
