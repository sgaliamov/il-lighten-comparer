using BenchmarkDotNet.Running;
using ILLightenComparer.Benchmarks.Benchmark;

namespace ILLightenComparer.Benchmarks
{
    public sealed class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<Benchmark.Comparer.LightStructComparerBenchmark>();
            BenchmarkRunner.Run<Benchmark.Comparer.RegularModelBenchmark>();
            BenchmarkRunner.Run<CompareIntegral>();
            BenchmarkRunner.Run<EqualityBenchmark>();
        }
    }
}
