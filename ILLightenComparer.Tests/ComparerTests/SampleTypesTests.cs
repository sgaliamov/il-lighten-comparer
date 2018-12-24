using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ILLightenComparer.Tests.Samples;
using ILLightenComparer.Tests.Samples.Comparers;
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
        public void Compare_Sample_Objects()
        {
            Test(typeof(SampleObject<>), typeof(SampleObjectComparer<>));
        }

        [Fact]
        public void Compare_Sample_Structs()
        {
            Test(typeof(SampleStruct<>), typeof(SampleStructComparer<>));
        }

        [Fact]
        public void Compare_Types_Directly()
        {
            foreach (var item in SampleTypes.Types)
            {
                var testMethod = GenericTests.GetTestMethod().MakeGenericMethod(item.Key);

                testMethod.Invoke(null, new object[] { item.Value, 10 });
            }
        }

        private static void TestCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types)
            {
                TestCollection(item.Key, item.Value, genericCollectionType, false);
            }
        }

        private static void TestNullableCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types.Where(x => x.Key.IsValueType))
            {
                TestCollection(item.Key, item.Value, genericCollectionType, true);
            }
        }

        private static void TestCollection(Type objectType, IComparer itemComparer, Type genericCollectionType, bool makeNullable)
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

            var comparer = constructor.Invoke(new object[] { itemComparer, false });

            var testMethod = GenericTests.GetTestMethod().MakeGenericMethod(collectionType);

            testMethod.Invoke(null, new[] { comparer, 100 });
        }

        private static void Test(Type objectGenericType, Type comparerGenericType)
        {
            foreach (var item in SampleTypes.Types)
            {
                var objectType = objectGenericType.MakeGenericType(item.Key);
                var comparerType = comparerGenericType.MakeGenericType(item.Key);
                var comparer = Activator.CreateInstance(comparerType, item.Value);

                var testMethod = GenericTests.GetTestMethod().MakeGenericMethod(objectType);

                testMethod.Invoke(null, new[] { comparer, 100 });
            }
        }
    }
}
