using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
using ILLightenComparer.Tests.Utilities;
using Xunit;

namespace ILLightenComparer.Tests.ComparerTests
{
    // todo: test with interface, abstract class and object
    public sealed class SampleTypesTests
    {
        [Fact]
        public void Compare_Arrays_Directly()
        {
            TestCollection();
        }

        [Fact]
        public void Compare_Arrays_Of_Nullables_Directly()
        {
            TestNullableCollection();
        }

        [Fact]
        public void Compare_Enumerables_Directly()
        {
            TestCollection(typeof(IEnumerable<>));
        }

        [Fact]
        public void Compare_Enumerables_Of_Nullables_Directly()
        {
            TestNullableCollection(typeof(IEnumerable<>));
        }

        [Fact]
        public void Compare_Nullable_Types_Directly()
        {
            foreach (var (type, comparer) in SampleTypes.Types.Where(x => x.Key.IsValueType))
            {
                var nullableComparer = Helper.CreateNullableComparer(type, comparer);
                var nullableType = type.MakeNullable();

                new GenericTests().GenericTest(nullableType, nullableComparer, false, Constants.SmallCount);
            }
        }

        [Fact]
        public void Compare_Types_Directly()
        {
            foreach (var (type, referenceComparer) in SampleTypes.Types)
            {
                new GenericTests().GenericTest(type, referenceComparer, false, Constants.SmallCount);
            }
        }

        [Fact(Skip = "remove me")]
        public void Test()
        {
            var comparer = new ComparersBuilder()
                           .DefineDefaultConfiguration(new ComparerSettings
                           {
                               IgnoreCollectionOrder = true
                           })
                           .For<sbyte[]>()
                           .GetComparer();

            var x = new sbyte[] { 0, -1, 0, -1, 0, -1, -1, -1, 0, 0 };
            var y = new sbyte[] { 0, 0, -1, -1, 0, 0, -128, -1, 127, 0 };

            var referenceComparer = new CollectionComparer<sbyte[], sbyte>(Comparer<sbyte>.Default, true);

            var expected = referenceComparer.Compare(x, y).Normalize();
            var actual = comparer.Compare(x, y).Normalize();

            actual.Should().Be(expected);
        }

        private static void TestCollection(Type genericCollectionType = null)
        {
            foreach (var (type, comparer) in SampleTypes.Types)
            {
                TestCollection(type, comparer, genericCollectionType, false, false);
                TestCollection(type, comparer, genericCollectionType, false, true);
            }
        }

        private static void TestNullableCollection(Type genericCollectionType = null)
        {
            foreach (var (type, comparer) in SampleTypes.Types.Where(x => x.Key.IsValueType))
            {
                TestCollection(type, comparer, genericCollectionType, true, false);
                TestCollection(type, comparer, genericCollectionType, true, true);
            }
        }

        private static void TestCollection(
            Type objectType,
            IComparer itemComparer,
            Type genericCollectionType,
            bool makeNullable,
            bool sort)
        {
            if (makeNullable)
            {
                itemComparer = Helper.CreateNullableComparer(objectType, itemComparer);
                objectType = objectType.MakeNullable();
            }

            var collectionType = genericCollectionType == null
                                     ? objectType.MakeArrayType()
                                     : genericCollectionType.MakeGenericType(objectType);

            var comparerType = typeof(CollectionComparer<,>).MakeGenericType(collectionType, objectType);
            var constructor = comparerType.GetConstructor(new[] { typeof(IComparer<>).MakeGenericType(objectType), typeof(bool) });
            var comparer = (IComparer)constructor.Invoke(new object[] { itemComparer, sort });

            new GenericTests().GenericTest(collectionType, comparer, sort, Constants.SmallCount);
        }
    }
}
