using BenchmarkDotNet.Running;
using ILLightenComparer.Benchmarks.Benchmark;
using ILLightenComparer.Benchmarks.Benchmark.Comparer;

namespace ILLightenComparer.Benchmarks
{
    public sealed class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<LightStructComparerBenchmark>();
            BenchmarkRunner.Run<RegularModelBenchmark>();
            BenchmarkRunner.Run<CompareIntegral>();
            BenchmarkRunner.Run<EqualityBenchmark>();
        }
    }
}
