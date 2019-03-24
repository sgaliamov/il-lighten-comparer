using System;
using BenchmarkDotNet.Running;
using ILLightenComparer.Benchmarks.Benchmark;

namespace ILLightenComparer.Benchmarks
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var comparer = new ComparerBuilder().GetComparer<MovieSampleObject>();

            var compare = comparer.Compare(new MovieSampleObject(), new MovieSampleObject());

            Console.WriteLine(compare);

            BenchmarkRunner.Run<ComparersBenchmark>();
        }
    }
}
