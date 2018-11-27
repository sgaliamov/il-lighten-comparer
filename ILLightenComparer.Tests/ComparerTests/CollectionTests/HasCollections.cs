using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.CollectionTests
{
    internal class HasCollections
    {
        public int[,,] ThreeDimensionalArray { get; set; }
        public int[][] ArrayOfArrays { get; set; }
        public string[] Strings { get; set; }
        public List<int> Integers { get; set; }
        public IEnumerable<DummyObject> SimpleItems { get; set; }
        public Dictionary<int, DummyObject> Dictionary { get; set; }
    }
}
