using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ILLightenComparer.Benchmark
{
    [MedianColumn]
    [RankColumn]
    public class CompareSubVsIfs
    {
        private readonly int[] _one = new int[N];
        private readonly int[] _other = new int[N];

        private int _out;
        private const int N = 10000;

        [GlobalSetup]
        public void Setup()
        {
            if (b)
            {
                throw new Exception();
            }

            b = true;

            var random = new Random();

            for (var i = 0; i < N; i++)
            {
                _one[i] = random.Next(int.MinValue, int.MaxValue);
                _other[i] = random.Next(int.MinValue, int.MaxValue);
            }
        }

        [Benchmark(Baseline = true)]
        public void Sub() // faster 6 times
        {
            for (var i = 0; i < N; i++)
            {
                _out = _one[i] - _other[i];
            }
        }

        [Benchmark]
        public void Ifs()
        {
            for (var i = 0; i < N; i++)
            {
                var one = _one[i];
                var other = _other[i];

                if (one == other)
                {
                    _out = 0;
                }

                if (one > other)
                {
                    _out = 1;
                }

                _out = -1;
            }
        }

        private static bool b;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CompareSubVsIfs>();

            Console.WriteLine(summary);
        }
    }
}
