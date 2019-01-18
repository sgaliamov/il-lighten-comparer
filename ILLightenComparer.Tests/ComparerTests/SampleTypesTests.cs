using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var item in SampleTypes.Types.Where(x => x.Key.IsValueType))
            {
                var nullableComparerType = typeof(NullableComparer<>).MakeGenericType(item.Key);
                var nullableComparer = (IComparer)Activator.CreateInstance(nullableComparerType, item.Value);
                var nullableType = typeof(Nullable<>).MakeGenericType(item.Key);

                GenericTests.GenericTest(nullableType, nullableComparer, false, Constants.SmallCount);
            }
        }

        [Fact]
        public void Compare_Types_Directly()
        {
            foreach (var item in SampleTypes.Types)
            {
                GenericTests.GenericTest(item.Key, item.Value, false, Constants.SmallCount);
            }
        }

        private static void TestCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types)
            {
                TestCollection(item.Key, item.Value, genericCollectionType, false, false);
                TestCollection(item.Key, item.Value, genericCollectionType, false, true);
            }
        }

        private static void TestNullableCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types.Where(x => x.Key.IsValueType))
            {
                TestCollection(item.Key, item.Value, genericCollectionType, true, false);
                TestCollection(item.Key, item.Value, genericCollectionType, true, true);
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
                var nullableComparer = typeof(NullableComparer<>).MakeGenericType(objectType);
                itemComparer = (IComparer)Activator.CreateInstance(nullableComparer, itemComparer);
                objectType = typeof(Nullable<>).MakeGenericType(objectType);
            }

            var collectionType = genericCollectionType == null
                                     ? objectType.MakeArrayType()
                                     : genericCollectionType.MakeGenericType(objectType);

            var comparerType = typeof(CollectionComparer<,>).MakeGenericType(collectionType, objectType);
            var constructor = comparerType.GetConstructor(new[] { typeof(IComparer<>).MakeGenericType(objectType), typeof(bool) });
            var comparer = (IComparer)constructor.Invoke(new object[] { itemComparer, sort });

            GenericTests.GenericTest(collectionType, comparer, sort, Constants.SmallCount);
        }
    }
}
