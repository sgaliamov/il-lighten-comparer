using System;
using ILLightenComparer.Benchmarks.Benchmark;

namespace ILLightenComparer.Benchmarks
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<CompareIntegral>();
            //BenchmarkRunner.Run<ComparersBenchmark>();

            var comparer = new ComparerBuilder().GetComparer<SampleObject>();

            var a = new SampleObject
            {
                Key = 1
            };
            var b = new SampleObject
            {
                Key = 2
            };

            var compare = comparer.Compare(a, b);
            Console.WriteLine(compare);
        }
    }
}
