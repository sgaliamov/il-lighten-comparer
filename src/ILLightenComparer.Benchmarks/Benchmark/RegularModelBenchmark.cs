using System.Collections.Generic;
using Nito.Comparers;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    public class RegularModelBenchmark : ComparersBenchmark<MovieSampleObject>
    {
        private static readonly IComparer<MovieSampleObject> Native = MovieSampleObject.Comparer;

        private static readonly IComparer<MovieSampleObject> ILLightenComparer
            = new ComparerBuilder(c =>
                  c.SetDefaultCyclesDetection(false)
                   .SetDefaultFieldsInclusion(false))
              .For<MovieSampleObject>()
              .GetComparer();

        private static readonly IComparer<MovieSampleObject> NitoComparer
            = Nito.Comparers.ComparerBuilder
                  .For<MovieSampleObject>()
                  .OrderBy(x => x.Actors)
                  .ThenBy(x => x.Genre)
                  .ThenBy(x => x.Id)
                  .ThenBy(x => x.Price)
                  .ThenBy(x => x.ReleaseDate)
                  .ThenBy(x => x.Title);

        public RegularModelBenchmark() : base(Native, ILLightenComparer, NitoComparer) { }
    }
}
