namespace ILLightenComparer.Tests.ComparerTests.HierarchyTests.Samples
{
    public class AbstractProperties
    {
        public AbstractNestedObject AbstractProperty { get; set; }
        public IAbstractNestedObject InterfaceProperty { get; set; }
        public BaseNestedObject NotSealedProperty { get; set; }
        public object ObjectProperty { get; set; }
    }
}
