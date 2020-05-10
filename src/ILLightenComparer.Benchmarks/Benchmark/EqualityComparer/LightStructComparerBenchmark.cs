using System.Collections.Generic;
using ILLightenComparer.Benchmarks.Models;
using Nito.Comparers;

namespace ILLightenComparer.Benchmarks.Benchmark.EqualityComparer
{
    public class LightStructComparerBenchmark : EqualityComparersBenchmark<LightStruct>
    {
        private static readonly IEqualityComparer<LightStruct> Native = LightStructEqualityComparer.Instance;

        private static readonly IEqualityComparer<LightStruct> ILLightenComparer = new ComparerBuilder(c => c
            .SetDefaultCyclesDetection(false))
            .For<LightStruct>()
            .GetEqualityComparer();

        private static readonly IEqualityComparer<LightStruct> NitoComparer = Nito.Comparers.ComparerBuilder
            .For<LightStruct>()
            .OrderBy(x => x.Key)
            .ThenBy(x => x.Value);

        public LightStructComparerBenchmark() : base(Native, ILLightenComparer, NitoComparer) { }
    }
}
