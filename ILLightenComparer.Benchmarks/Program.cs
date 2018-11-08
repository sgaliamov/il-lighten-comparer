using System;
using BenchmarkDotNet.Running;
using ILLightenComparer.Benchmarks.Benchmark;

namespace ILLightenComparer.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CompareIntegral>();

            Console.WriteLine(summary);
        }
    }
}
