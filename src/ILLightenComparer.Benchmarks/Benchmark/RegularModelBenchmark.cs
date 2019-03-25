using System.Collections.Generic;
using Nito.Comparers;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    public class RegularModelBenchmark : ComparersBenchmark<MovieObject>
    {
        private static readonly IComparer<MovieObject> Manual = MovieObject.Comparer;

        private static readonly IComparer<MovieObject> ILLightenComparer
            = new ComparerBuilder(c =>
                  c.SetDefaultCyclesDetection(false)
                   .SetDefaultFieldsInclusion(false))
              .For<MovieObject>()
              .GetComparer();

        private static readonly IComparer<MovieObject> NitoComparer
            = Nito.Comparers.ComparerBuilder
                  .For<MovieObject>()
                  .OrderBy(x => x.Actors)
                  .ThenBy(x => x.Genre)
                  .ThenBy(x => x.Id)
                  .ThenBy(x => x.Price)
                  .ThenBy(x => x.ReleaseDate)
                  .ThenBy(x => x.Title);

        public RegularModelBenchmark() : base(Manual, ILLightenComparer, NitoComparer) { }
    }
}
