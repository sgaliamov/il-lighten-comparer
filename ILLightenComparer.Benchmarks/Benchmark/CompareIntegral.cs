using System;
using BenchmarkDotNet.Attributes;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    [MedianColumn]
    [RankColumn]
    public class CompareIntegral
    {
        private readonly int[] _one = new int[N];
        private readonly int[] _other = new int[N];

        // ReSharper disable once NotAccessedField.Local
        private int _out;
        private const int N = 10000;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random();

            for (var i = 0; i < N; i++)
            {
                _one[i] = random.Next(int.MinValue, int.MaxValue);
                _other[i] = random.Next(int.MinValue, int.MaxValue);
            }
        }

        [Benchmark(Baseline = true)]
        public void Sub() // fastest
        {
            for (var i = 0; i < N; i++)
            {
                _out = _one[i] - _other[i];
            }
        }

        [Benchmark]
        public void CompareTo()
        {
            for (var i = 0; i < N; i++)
            {
                _out = _one[i].CompareTo(_other[i]);
            }
        }

        [Benchmark]
        public void Ifs()
        {
            for (var i = 0; i < N; i++)
            {
                var one = _one[i];
                var other = _other[i];

                _out = 0;

                if (one < other)
                {
                    _out = -1;
                }
                else if (one > other)
                {
                    _out = 1;
                }
            }
        }
    }
}
