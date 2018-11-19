using System.Collections.Generic;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;
using Nito.Comparers;

namespace ILLightenComparer.Benchmarks.Benchmark
{
    [MedianColumn]
    [RankColumn]
    public class ComparersBenchmark
    {
        private const int N = 1000;

        private static readonly IComparer<TestObject> Native = TestObject.TestObjectComparer;

        private static readonly IComparer<TestObject> ILLightenComparer =
            new ComparersBuilder().CreateComparer<TestObject>();

        private static readonly IComparer<TestObject> NitoComparer = ComparerBuilder.For<TestObject>().Default();

        private static readonly Fixture Fixture = FixtureBuilder.GetInstance();

        private readonly TestObject[] _one = new TestObject[N];
        private readonly TestObject[] _other = new TestObject[N];

        // ReSharper disable once NotAccessedField.Local
        private int _out;

        [GlobalSetup]
        public void Setup()
        {
            for (var i = 0; i < N; i++)
            {
                _one[i] = Fixture.Create<TestObject>();
                _other[i] = Fixture.Create<TestObject>();
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
        public void Native_Comparer()
        {
            for (var i = 0; i < N; i++)
            {
                _out = Native.Compare(_one[i], _other[i]);
            }
        }

        [Benchmark]
        public void Ifs()
        {
            for (var i = 0; i < N; i++)
            {
                _out = NitoComparer.Compare(_one[i], _other[i]);
            }
        }
    }
}
