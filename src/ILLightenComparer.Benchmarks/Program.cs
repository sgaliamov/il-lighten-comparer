using BenchmarkDotNet.Running;
using ILLightenComparer.Benchmarks.Benchmark;

namespace ILLightenComparer.Benchmarks
{
    public sealed class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<LightStructBenchmark>();
            BenchmarkRunner.Run<RegularModelBenchmark>();
            BenchmarkRunner.Run<CompareIntegral>();
            BenchmarkRunner.Run<EqualityBenchmark>();
        }
    }
}
