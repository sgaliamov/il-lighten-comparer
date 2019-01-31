using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    public sealed class SampleCollectionMembersTests
    {
        [Fact]
        public void Compare_Sample_Objects()
        {
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false, false, false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false, false, true);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, false, false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, false, true);
        }

        [Fact]
        public void Compare_Sample_Objects_Ignore_Order()
        {
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false, true, false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), false, true, true);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, true, false);
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>), true, true, true);
        }

        [Fact]
        public void Compare_Sample_Structs()
        {
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false, false, false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false, false, true);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, false, false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, false, true);
        }

        [Fact]
        public void Compare_Sample_Structs_Ignore_Order()
        {
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false, true, false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), false, true, true);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, true, false);
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>), true, true, true);
        }

        [Fact]
        public void Ignoring_Order_Do_Not_Add_Side_Effect()
        {
            Ignoring_Order_Do_Not_Add_Side_Effect_For(typeof(SampleObject<>), typeof(int));
            Ignoring_Order_Do_Not_Add_Side_Effect_For(typeof(SampleStruct<>), typeof(int));
            Ignoring_Order_Do_Not_Add_Side_Effect_For(typeof(int), null);
            Ignoring_Order_Do_Not_Add_Side_Effect_For(typeof(SampleObject<>), typeof(SampleObject<int>));
            Ignoring_Order_Do_Not_Add_Side_Effect_For(typeof(SampleStruct<>), typeof(SampleObject<int>));
        }

        private static void Ignoring_Order_Do_Not_Add_Side_Effect_For(Type sampleType, Type memberType)
        {
            var type = memberType == null
                           ? sampleType
                           : sampleType.MakeGenericType(memberType);

            var method = typeof(SampleCollectionMembersTests)
                         .GetGenericMethod(
                             nameof(Ignoring_Order_Do_Not_Add_Side_Effect_For),
                             BindingFlags.NonPublic | BindingFlags.Static)
                         .MakeGenericMethod(type);

            method.Invoke(null, null);
        }

        private static void Ignoring_Order_Do_Not_Add_Side_Effect_For<TElement>()
        {
            var comparer = new ComparersBuilder()
                           .DefineDefaultConfiguration(new ComparerSettings { IgnoreCollectionOrder = true })
                           .GetComparer<TElement[]>();

            var fixture = FixtureBuilder.GetInstance();
            var sample = fixture.Create<TElement[]>();
            var clone = sample.DeepClone();

            comparer.Compare(sample, fixture.Create<TElement[]>()).Should().NotBe(0);

            sample.ShouldBeSameOrder(clone);
        }

        private static void Test(Type genericSampleType, Type genericSampleComparer, bool useArrays, bool sort, bool makeNullable)
        {
            Parallel.ForEach(
                SampleTypes.Types,
                item =>
                {
                    var (type, referenceComparer) = item;
                    var itemComparer = referenceComparer;
                    var objectType = type;

                    if (makeNullable && type.IsValueType)
                    {
                        itemComparer = Helper.CreateNullableComparer(objectType, itemComparer);
                        objectType = objectType.MakeNullable();
                    }

                    var collectionType = useArrays
                                             ? objectType.MakeArrayType()
                                             : typeof(List<>).MakeGenericType(objectType);

                    var sampleType = genericSampleType.MakeGenericType(collectionType);

                    var collectionComparerType = typeof(CollectionComparer<>).MakeGenericType(objectType);
                    var collectionComparer = Activator.CreateInstance(collectionComparerType, itemComparer, sort);

                    var sampleComparerType = genericSampleComparer.MakeGenericType(collectionType);
                    var sampleComparer = (IComparer)Activator.CreateInstance(sampleComparerType, collectionComparer);

                    new GenericTests().GenericTest(sampleType, sampleComparer, sort, Constants.SmallCount);
                });
        }
    }
}
