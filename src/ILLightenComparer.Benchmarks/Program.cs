using BenchmarkDotNet.Running;
using ILLightenComparer.Benchmarks.Benchmark;
using ILLightenComparer.Benchmarks.Benchmark.Comparer;
using ILLightenComparer.Benchmarks.Benchmark.EqualityComparer;

namespace ILLightenComparer.Benchmarks
{
    public sealed class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<LightStructComparerBenchmark>();
            BenchmarkRunner.Run<RegularModelComparerBenchmark>();

            BenchmarkRunner.Run<RegularModelEqualityBenchmark>();
            BenchmarkRunner.Run<RegularModelEqualityBenchmark>();

            BenchmarkRunner.Run<LightStructEqualityBenchmark>();
            BenchmarkRunner.Run<LightStructHashBenchmark>();

            BenchmarkRunner.Run<CompareIntegral>();
            BenchmarkRunner.Run<EqualityBenchmark>();
        }
    }
}
