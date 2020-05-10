using System.Collections.Generic;
using Nito.Comparers;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    public class LightStructBenchmark : ComparersBenchmark<LightStruct>
    {
        private static readonly IComparer<LightStruct> Native = LightStruct.Comparer;

        private static readonly IComparer<LightStruct> ILLightenComparer = new ComparerBuilder(c => c
            .SetDefaultCyclesDetection(false))
            .For<LightStruct>()
            .GetComparer();

        private static readonly IComparer<LightStruct> NitoComparer = Nito.Comparers.ComparerBuilder
            .For<LightStruct>()
            .OrderBy(x => x.Key)
            .ThenBy(x => x.Value);

        public LightStructBenchmark() : base(Native, ILLightenComparer, NitoComparer) { }
    }
}
