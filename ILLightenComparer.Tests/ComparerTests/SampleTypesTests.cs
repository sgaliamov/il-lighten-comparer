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
                TestCollection(objectType, genericCollectionType, item.Value);
            }
        }

        private void TestNullableCollection(Type genericCollectionType = null)
        {
            foreach (var item in SampleTypes.Types)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(item.Key);
                TestCollection(nullableType, genericCollectionType, item.Value);
            }
        }

        private void TestCollection(Type objectType, Type genericCollectionType, IComparer itemComparer)
        {
            var collectionType = genericCollectionType == null
                                     ? objectType.MakeArrayType()
                                     : genericCollectionType.MakeGenericType(objectType);
            var comparerType = typeof(CollectionComparer<,>).MakeGenericType(collectionType, objectType);
            var comparer = Activator.CreateInstance(comparerType, itemComparer, false);
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
