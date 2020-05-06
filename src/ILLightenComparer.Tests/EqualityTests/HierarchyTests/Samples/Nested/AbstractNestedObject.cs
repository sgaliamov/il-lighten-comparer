using ILLightenComparer.Tests.EqualityComparers;

namespace ILLightenComparer.Tests.EqualityTests.HierarchyTests.Samples.Nested
{
    public abstract class AbstractNestedObject : INestedObject
    {
        public string Text { get; set; }

        public override bool Equals(object obj) => Equals((AbstractNestedObject)obj);

        public bool Equals(AbstractNestedObject other) => other != null && Text == other.Text;

        public override int GetHashCode() => HashCodeCombiner.Combine(Text);
    }
}
