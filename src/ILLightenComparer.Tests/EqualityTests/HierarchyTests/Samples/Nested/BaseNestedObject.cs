using System;
using ILLightenComparer.Tests.EqualityComparers;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.EqualityTests.HierarchyTests.Samples.Nested
{
    public class BaseNestedObject : AbstractNestedObject
    {
        public EnumSmall? Key { get; set; }

        public override bool Equals(object obj) => Equals((BaseNestedObject)obj);

        public bool Equals(BaseNestedObject other) => other != null && base.Equals(other) && Key == other.Key;

        public override int GetHashCode() => HashCodeCombiner.Combine<object>(Text, Key);
    }
}
