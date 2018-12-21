using System;
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

        private MethodInfo GetTestMethod()
        {
            return typeof(SampleTypesTests)
                   .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                   .Single(x => x.Name == nameof(GenericTest) && x.IsGenericMethodDefinition);
        }

        private void GenericTest<T>(IComparer<T> referenceComparer, int times)
        {
            T Create()
            {
                if ((!typeof(T).IsValueType || typeof(T).IsNullable()) && _random.NextDouble() < 0.1)
                {
                    return default;
                }

                return _fixture.Create<T>();
            }

            if (referenceComparer == null) { referenceComparer = Comparer<T>.Default; }

            var comparer = new ComparersBuilder().GetComparer<T>();

            for (var i = 0; i < times; i++)
            {
                var x = Create();
                var y = Create();

                var expected = referenceComparer.Compare(x, y).Normalize();
                var actual = comparer.Compare(x, y).Normalize();

                var message = $"{typeof(T).DisplayName()} should be supported.\nx: {x},\ny: {y}";
                actual.Should().Be(expected, message);
            }
        }

        private readonly Fixture _fixture = FixtureBuilder.GetInstance();
        private readonly Random _random = new Random();
    }
}
