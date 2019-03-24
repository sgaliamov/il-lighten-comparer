using System;
using System.Collections.Generic;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using Nito.Comparers;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    [MedianColumn]
    [RankColumn]
    public class ComparersBenchmark
    {
        private const int N = 1000;

        private static readonly Fixture Fixture = new Fixture();

        private static readonly IComparer<MovieSampleObject> Native = MovieSampleObject.Comparer;

        private static readonly IComparer<MovieSampleObject> ILLightenComparer
            = new ComparerBuilder()
              .For<MovieSampleObject>()
              .GetComparer();

        private static readonly IComparer<MovieSampleObject> NitoComparer
            = Nito.Comparers.ComparerBuilder
                  .For<MovieSampleObject>()
                  .Default();

        private readonly MovieSampleObject[] _one = new MovieSampleObject[N];
        private readonly MovieSampleObject[] _other = new MovieSampleObject[N];

        // ReSharper disable once NotAccessedField.Local
        private int _out;

        [GlobalSetup]
        public void Setup()
        {
            int Normalize(int value)
            {
                if (value >= 1)
                {
                    return 1;
                }

                if (value <= -1)
                {
                    return -1;
                }

                return 0;
            }

            for (var i = 0; i < N; i++)
            {
                _one[i] = Fixture.Create<MovieSampleObject>();
                _other[i] = Fixture.Create<MovieSampleObject>();

                var compare = Normalize(Native.Compare(_one[i], _other[i]));

                if (compare != Normalize(ILLightenComparer.Compare(_one[i], _other[i])))
                {
                    throw new InvalidOperationException("ILLightenComparer comparer is broken.");
                }

                if (compare != Normalize(NitoComparer.Compare(_one[i], _other[i])))
                {
                    throw new InvalidOperationException("Nito comparer is broken.");
                }
            }
        }

        [Benchmark(Baseline = true)]
        public void IL_Comparer() // fastest
        {
            for (var i = 0; i < N; i++)
            {
                _out = ILLightenComparer.Compare(_one[i], _other[i]);
            }
        }

        [Benchmark]
        public void Nito_Comparer()
        {
            for (var i = 0; i < N; i++)
            {
                _out = NitoComparer.Compare(_one[i], _other[i]);
            }
        }

        [Benchmark]
        public void Native_Comparer()
        {
            for (var i = 0; i < N; i++)
            {
                _out = Native.Compare(_one[i], _other[i]);
            }
        }
    }
}
