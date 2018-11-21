using System;
using System.Collections.Generic;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Benchmarks.Benchmark
{
//          Method |     Mean |     Error |    StdDev |   Median | Ratio | Rank |
//---------------- |---------:|----------:|----------:|---------:|------:|-----:|
//     IL_Comparer | 16.91 us | 0.1019 us | 0.0953 us | 16.90 us |  1.06 |    2 |
// Native_Comparer | 15.92 us | 0.0932 us | 0.0872 us | 15.93 us |  1.00 |    1 |

    [MedianColumn]
    [RankColumn]
    public class ComparersBenchmark
    {
        private const int N = 1000;

        private static readonly IComparer<TestObject> Native = TestObject.TestObjectComparer;

        private static readonly IComparer<TestObject> ILLightenComparer =
            new ComparersBuilder()
                .SetConfiguration(new CompareConfiguration
                {
                    IncludeFields = true
                })
                .CreateComparer<TestObject>();

        //private static readonly IComparer<TestObject> NitoComparer = ComparerBuilder
        //                                                             .For<TestObject>()
        //                                                             .OrderBy(x => x.BooleanField)
        //                                                             .ThenBy(x => x.ByteField)
        //                                                             .ThenBy(x => x.CharField)
        //                                                             .ThenBy(x => x.DecimalField)
        //                                                             .ThenBy(x => x.DoubleField)
        //                                                             .ThenBy(x => x.EnumField)
        //                                                             .ThenBy(x => x.Int16Field)
        //                                                             .ThenBy(x => x.Int32Field)
        //                                                             .ThenBy(x => x.Int64Field)
        //                                                             .ThenBy(x => x.SByteField)
        //                                                             .ThenBy(x => x.SingleField)
        //                                                             .ThenBy(x => x.StringField)
        //                                                             .ThenBy(x => x.UInt16Field)
        //                                                             .ThenBy(x => x.UInt32Field)
        //                                                             .ThenBy(x => x.UInt64Field)
        //                                                             .ThenBy(x => x.BooleanProperty)
        //                                                             .ThenBy(x => x.ByteProperty)
        //                                                             .ThenBy(x => x.CharProperty)
        //                                                             .ThenBy(x => x.DecimalProperty)
        //                                                             .ThenBy(x => x.DoubleProperty)
        //                                                             .ThenBy(x => x.EnumProperty)
        //                                                             .ThenBy(x => x.Int16Property)
        //                                                             .ThenBy(x => x.Int32Property)
        //                                                             .ThenBy(x => x.Int64Property)
        //                                                             .ThenBy(x => x.SByteProperty)
        //                                                             .ThenBy(x => x.SingleProperty)
        //                                                             .ThenBy(x => x.StringProperty)
        //                                                             .ThenBy(x => x.UInt16Property)
        //                                                             .ThenBy(x => x.UInt32Property)
        //                                                             .ThenBy(x => x.UInt64Property);

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

                var compare = Normalize(Native.Compare(_one[i], _other[i]));

                if (compare != Normalize(ILLightenComparer.Compare(_one[i], _other[i])))
                {
                    throw new InvalidOperationException("ILLightenComparer comparer is broken.");
                }

                //if (compare != Normalize(NitoComparer.Compare(_one[i], _other[i])))
                //{
                //    throw new InvalidOperationException("Nito comparer is broken.");
                //}
            }
        }

        //[Benchmark]
        //public void Nito_Comparer()
        //{
        //    for (var i = 0; i < N; i++)
        //    {
        //        _out = NitoComparer.Compare(_one[i], _other[i]);
        //    }
        //}

        [Benchmark]
        public void IL_Comparer() // fastest
        {
            for (var i = 0; i < N; i++)
            {
                _out = ILLightenComparer.Compare(_one[i], _other[i]);
            }
        }

        [Benchmark(Baseline = true)]
        public void Native_Comparer()
        {
            for (var i = 0; i < N; i++)
            {
                _out = Native.Compare(_one[i], _other[i]);
            }
        }

        private static int Normalize(int value)
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
    }
}
