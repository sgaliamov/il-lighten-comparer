using System;
using System.Collections;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleCollectionMembersTests
    {
        [Fact]
        public void Compare_Array_Of_Arrays()
        {
            var builder = new ComparersBuilder();

            Assert.Throws<NotSupportedException>(() => builder.For<SampleObject<int[][]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<SampleObject<int[][,]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<SampleObject<int[,]>>().GetComparer());

            Assert.Throws<NotSupportedException>(() => builder.For<SampleStruct<int[][]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<SampleStruct<int[][,]>>().GetComparer());
            Assert.Throws<NotSupportedException>(() => builder.For<SampleStruct<int[,]>>().GetComparer());
        }

        [Fact]
        public void Compare_Enumerable_Of_Enumerables()
        {
            var builder = new ComparersBuilder();

            Assert.Throws<NotSupportedException>(
                () => builder.For<SampleObject<IEnumerable<IEnumerable<int>>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => builder.For<SampleObject<IEnumerable<int[,]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => builder.For<SampleObject<IEnumerable<int[]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => builder.For<SampleObject<IEnumerable<int>[]>>().GetComparer());

            Assert.Throws<NotSupportedException>(
                () => builder.For<SampleStruct<IEnumerable<IEnumerable<int>>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => builder.For<SampleStruct<IEnumerable<int[,]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => builder.For<SampleStruct<IEnumerable<int[]>>>().GetComparer());
            Assert.Throws<NotSupportedException>(
                () => builder.For<SampleStruct<IEnumerable<int>[]>>().GetComparer());
        }

        [Fact]
        public void Compare_Sample_Objects()
        {
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false, false, false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false, false, true);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false, true, false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false, true, true);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, false, false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, false, true);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, true, false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, true, true);
        }

        [Fact]
        public void Compare_Sample_Structs()
        {
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false, false, false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false, false, true);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false, true, false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false, true, true);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, false, false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, false, true);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, true, false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, true, true);
        }

        private static void Test(Type genericSampleType, Type genericSampleComparer, bool useArrays, bool sort, bool makeNullable)
        {
            foreach (var item in SampleTypes.Types)
            {
                var itemComparer = item.Value;
                var objectType = item.Key;
                if (makeNullable && item.Key.IsValueType)
                {
                    var nullableComparerType = typeof(NullableComparer<>).MakeGenericType(item.Key);
                    itemComparer = (IComparer)Activator.CreateInstance(nullableComparerType, itemComparer);
                    objectType = typeof(Nullable<>).MakeGenericType(objectType);
                }

                var collectionType = useArrays
                                         ? objectType.MakeArrayType()
                                         : typeof(IEnumerable<>).MakeGenericType(objectType);

                var sampleType = genericSampleType.MakeGenericType(collectionType);

                var collectionComparerType = typeof(CollectionComparer<,>).MakeGenericType(collectionType, objectType);
                var collectionComparer = Activator.CreateInstance(collectionComparerType, itemComparer, sort);

                var sampleComparerType = genericSampleComparer.MakeGenericType(collectionType);
                var sampleComparer = (IComparer)Activator.CreateInstance(sampleComparerType, collectionComparer);

                GenericTests.GenericTest(sampleType, sampleComparer, sort, Constants.SmallCount);
            }
        }
    }
}
