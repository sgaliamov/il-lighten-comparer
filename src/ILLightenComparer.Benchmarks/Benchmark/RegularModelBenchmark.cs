using System.Collections.Generic;
using Nito.Comparers;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    public class RegularModelBenchmark : ComparersBenchmark<MovieModel>
    {
        private static readonly IComparer<MovieModel> Manual = MovieModel.Comparer;

        private static readonly IComparer<MovieModel> ILLightenComparer
            = new ComparerBuilder(c =>
                  c.SetDefaultCyclesDetection(false)
                   .SetDefaultFieldsInclusion(false))
              .For<MovieModel>()
              .GetComparer();

        private static readonly IComparer<MovieModel> NitoComparer
            = Nito.Comparers.ComparerBuilder
                  .For<MovieModel>()
                  .OrderBy(x => x.Actors)
                  .ThenBy(x => x.Genre)
                  .ThenBy(x => x.Id)
                  .ThenBy(x => x.Price)
                  .ThenBy(x => x.ReleaseDate)
                  .ThenBy(x => x.Title);

        public RegularModelBenchmark() : base(Manual, ILLightenComparer, NitoComparer) { }
    }
}
