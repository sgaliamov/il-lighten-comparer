using ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples.Nested;

namespace ILLightenComparer.Tests.EqualityTests.HierarchyTests.Samples
{
    public class AbstractMembers
    {
        public INestedObject InterfaceField;
        public object ObjectField;

        public AbstractNestedObject AbstractProperty { get; set; }
        public BaseNestedObject NotSealedProperty { get; set; }
    }
}
