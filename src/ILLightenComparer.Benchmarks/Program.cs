using BenchmarkDotNet.Running;
using ILLightenComparer.Benchmarks.Benchmark;

namespace ILLightenComparer.Benchmarks
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<LightStructBenchmark>();
            BenchmarkRunner.Run<RegularModelBenchmark>();
        }
    }
}
