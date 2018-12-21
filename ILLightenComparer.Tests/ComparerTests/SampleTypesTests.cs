using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
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
                var testMethod = GetTestMethod().MakeGenericMethod(item.Key);

                testMethod.Invoke(this, new object[] { item.Value, 10 });
            }
        }

        private void TestCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types)
            {
                var objectType = item.Key;
                TestCollection(objectType, item.Value, genericCollectionType, false);
            }
        }

        private void TestNullableCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types.Where(x => x.Key.IsValueType))
            {
                TestCollection(item.Key, item.Value, genericCollectionType, true);
            }
        }

        private void TestCollection(Type objectType, IComparer itemComparer, Type genericCollectionType, bool makeNullable)
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

            var testMethod = GetTestMethod().MakeGenericMethod(collectionType);

            testMethod.Invoke(this, new[] { comparer, 100 });
        }

        private void Test(Type objectGenericType, Type comparerGenericType)
        {
            foreach (var item in SampleTypes.Types)
            {
                var objectType = objectGenericType.MakeGenericType(item.Key);
                var comparerType = comparerGenericType.MakeGenericType(item.Key);
                var comparer = Activator.CreateInstance(comparerType, item.Value);

                var testMethod = GetTestMethod().MakeGenericMethod(objectType);

                testMethod.Invoke(this, new[] { comparer, 100 });
            }
        }

        private void GenericTest<T>(IComparer<T> referenceComparer, int times)
        {
            var type = typeof(T);

            T Create()
            {
                if ((!type.IsValueType || type.IsNullable()) && _random.NextDouble() < 0.1)
                {
                    return default;
                }

                var result = _fixture.Create<T>();
                if (result is IList list)
                {
                    var count = Math.Max(list.Count / 10, 1);
                    for (var i = 0; i < count; i++)
                    {
                        list[_random.Next(list.Count)] = null;
                    }
                }

                return result;
            }

            if (referenceComparer == null) { referenceComparer = Comparer<T>.Default; }

            var comparer = new ComparersBuilder().GetComparer<T>();

            for (var i = 0; i < times; i++)
            {
                var x = Create();
                var y = Create();

                var expected = referenceComparer.Compare(x, y).Normalize();
                var actual = comparer.Compare(x, y).Normalize();

                var message = $"{type.DisplayName()} should be supported.\nx: {x},\ny: {y}";
                actual.Should().Be(expected, message);
            }
        }

        private MethodInfo GetTestMethod()
        {
            return typeof(SampleTypesTests)
                   .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                   .Single(x => x.Name == nameof(GenericTest) && x.IsGenericMethodDefinition);
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
        private readonly Random _random = new Random();
    }
}
