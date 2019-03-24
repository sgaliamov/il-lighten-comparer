using System;
using BenchmarkDotNet.Running;
using ILLightenComparer.Benchmarks.Benchmark;

namespace ILLightenComparer.Benchmarks
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var compare = new ComparerBuilder(c =>
                              c.SetDefaultCyclesDetection(false)
                               .SetDefaultFieldsInclusion(false))
                          .GetComparer<MovieSampleObject>()
                          .Compare(new MovieSampleObject(), new MovieSampleObject());

            Console.WriteLine(compare);

            BenchmarkRunner.Run<ComparersBenchmark>();
        }
    }
}
