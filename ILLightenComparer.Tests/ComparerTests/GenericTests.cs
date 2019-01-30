using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using FluentAssertions;
using Force.DeepCloner;
using ILLightenComparer.Tests.Utilities;

namespace ILLightenComparer.Tests.ComparerTests
{
    internal sealed class GenericTests
    {
        private static readonly Random Random = new Random();
        private static readonly Fixture Fixture = FixtureBuilder.GetInstance();
        private readonly ComparersBuilder _comparersBuilder;

        public GenericTests(ComparersBuilder comparersBuilder = null)
        {
            _comparersBuilder = comparersBuilder;
        }

        public void GenericTest(Type type, IComparer referenceComparer, bool sort, int times)
        {
            var method = type.GetOrAddProperty(
                nameof(GenericTest),
                () =>
                {
                    var methodInfo = GetTestMethod(type);

                    return (Action<IComparerProvider, IComparer, int>)methodInfo.CreateDelegate(typeof(Action<IComparerProvider, IComparer, int>));
                });

            var builder = _comparersBuilder ?? new ComparersBuilder();
            builder.DefineDefaultConfiguration(new ComparerSettings { IgnoreCollectionOrder = sort });

            method(builder, referenceComparer, times);
        }

        private static MethodInfo GetTestMethod(Type objType)
        {
            return typeof(GenericTests)
                   .GetGenericMethod(nameof(Test), BindingFlags.Static | BindingFlags.NonPublic)
                   .MakeGenericMethod(objType);
        }

        private static void Test<T>(IComparerProvider comparersBuilder, IComparer referenceComparer, int times)
        {
            if (referenceComparer == null) { referenceComparer = Comparer<T>.Default; }

            var type = typeof(T);

            var typedComparer = comparersBuilder.GetComparer<T>();
            var basicComparer = comparersBuilder.GetComparer(type);

            Comparison_Of_Null_With_Object_Produces_Negative_Value(referenceComparer, typedComparer, basicComparer);
            Comparison_Of_Object_With_Null_Produces_Positive_Value(referenceComparer, typedComparer, basicComparer);
            Comparison_When_Both_Null_Produces_0(referenceComparer, typedComparer, basicComparer);
            Comparison_With_Itself_Produces_0(referenceComparer, typedComparer, basicComparer);
            Comparison_With_Same_Produces_0(referenceComparer, typedComparer, basicComparer);

            Comparisons_Work_Identical(referenceComparer, typedComparer, basicComparer, times);
            Sorting_Must_Work_The_Same_As_For_Reference_Comparer(referenceComparer, typedComparer, basicComparer, Constants.BigCount);
            Mutate_Class_Members_And_Test_Comparison(referenceComparer, typedComparer, basicComparer);
        }

        private static void Comparison_Of_Null_With_Object_Produces_Negative_Value<T>(
            IComparer referenceComparer,
            IComparer<T> typedComparer,
            IComparer basicComparer)
        {
            var obj = Fixture.Create<T>();
            if (typeof(T).IsClass)
            {
                referenceComparer.Compare(default, obj).Should().BeNegative();
                typedComparer.Compare(default, obj).Should().BeNegative();
            }

            basicComparer.Compare(null, obj).Should().BeNegative();
        }

        private static void Comparison_Of_Object_With_Null_Produces_Positive_Value<T>(
            IComparer referenceComparer,
            IComparer<T> typedComparer,
            IComparer basicComparer)
        {
            var obj = Fixture.Create<T>();
            if (typeof(T).IsClass)
            {
                referenceComparer.Compare(obj, default).Should().BePositive();
                typedComparer.Compare(obj, default).Should().BePositive();
            }

            basicComparer.Compare(obj, null).Should().BePositive();
        }

        private static void Comparison_When_Both_Null_Produces_0<T>(
            IComparer referenceComparer,
            IComparer<T> typedComparer,
            IComparer basicComparer)
        {
            if (typeof(T).IsClass)
            {
                referenceComparer.Compare(default, default).Should().Be(0);
                typedComparer.Compare(default, default).Should().Be(0);
            }

            basicComparer.Compare(null, null).Should().Be(0);
        }

        private static void Comparison_With_Itself_Produces_0<T>(
            IComparer referenceComparer,
            IComparer<T> typedComparer,
            IComparer basicComparer)
        {
            var obj = Fixture.Create<T>();

            referenceComparer.Compare(obj, obj).Should().Be(0);
            typedComparer.Compare(obj, obj).Should().Be(0);
            basicComparer.Compare(obj, obj).Should().Be(0);
        }

        private static void Comparison_With_Same_Produces_0<T>(
            IComparer referenceComparer,
            IComparer<T> typedComparer,
            IComparer basicComparer)
        {
            var obj = Fixture.Create<T>();
            var clone = obj.DeepClone();

            referenceComparer.Compare(obj, clone).Should().Be(0);
            typedComparer.Compare(obj, clone).Should().Be(0);
            basicComparer.Compare(obj, clone).Should().Be(0);
        }

        private static void Mutate_Class_Members_And_Test_Comparison<T>(
            IComparer referenceComparer,
            IComparer<T> typedComparer,
            IComparer basicComparer)
        {
            if (typeof(T).IsValueType) { return; }

            if (typeof(T).GetGenericInterface(typeof(IEnumerable<>)) != null) { return; }

            var original = Fixture.Create<T>();
            foreach (var mutant in Fixture.CreateMutants(original))
            {
                referenceComparer.Compare(mutant, original).Should().NotBe(0);
                basicComparer.Compare(mutant, original).Should().NotBe(0);
                typedComparer.Compare(mutant, original).Should().NotBe(0);
            }
        }

        private static void Sorting_Must_Work_The_Same_As_For_Reference_Comparer<T>(
            IComparer referenceComparer,
            IComparer<T> typedComparer,
            IComparer basicComparer,
            int count)
        {
            var original = CreateMany<T>(count).ToArray();

            var copy0 = original.DeepClone();
            var copy1 = original.DeepClone();
            var copy2 = original.DeepClone();

            Array.Sort(copy0, referenceComparer);
            Array.Sort(copy1, typedComparer);
            Array.Sort(copy2, basicComparer);

            copy1.ShouldBeSameOrder(copy0);
            copy1.ShouldBeSameOrder(copy0);
        }

        private static void Comparisons_Work_Identical<T>(
            IComparer referenceComparer,
            IComparer<T> typedComparer,
            IComparer basicComparer,
            int times)
        {
            var type = typeof(T);
            for (var i = 0; i < times; i++)
            {
                var x = Create<T>();
                var y = Create<T>();

                var expected = referenceComparer.Compare(x, y).Normalize();
                var actual1 = typedComparer.Compare(x, y).Normalize();
                var actual2 = basicComparer.Compare(x, y).Normalize();

                var message = $"{type.DisplayName()} should be supported.\n"
                              + $"x: {x.ToJson()},\n"
                              + $"y: {y.ToJson()}";
                actual1.Should().Be(expected, message);
                actual2.Should().Be(expected, message);
            }
        }

        private static T Create<T>()
        {
            var type = typeof(T);

            if ((!type.IsValueType || type.IsNullable()) && Random.NextDouble() < Constants.NullProbability)
            {
                return default;
            }

            var result = Fixture.Create<T>();

            if (type.IsValueType)
            {
                return result;
            }

            if (type.IsArray && result is IList list)
            {
                return (T)AppendNulls(list); // todo: test
            }

            var genericInterface = type.GetGenericInterface(typeof(IEnumerable<>));
            if (genericInterface != null)
            {
                return AppendNulls(result, genericInterface);
            }

            return result;
        }

        private static IList AppendNulls(IList list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (Random.NextDouble() < Constants.NullProbability)
                {
                    list[i] = null;
                }
            }

            return list;
        }

        private static T AppendNulls<T>(T result, Type genericInterface)
        {
            var elementType = genericInterface.GetGenericArguments()[0];
            if (elementType.IsValueType && !elementType.IsNullable())
            {
                return result;
            }

            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod(nameof(List<object>.Add), new[] { elementType });
            var asEnumerableMethod = typeof(Enumerable)
                                     .GetGenericMethod(nameof(Enumerable.AsEnumerable), BindingFlags.Static | BindingFlags.Public)
                                     .MakeGenericMethod(elementType);

            foreach (var item in (IEnumerable)result)
            {
                var parameters = Random.NextDouble() < Constants.NullProbability
                                     ? new[] { (object)null }
                                     : new[] { item };

                addMethod.Invoke(list, parameters);
            }

            return (T)asEnumerableMethod.Invoke(null, new[] { list });
        }

        private static IEnumerable<T> CreateMany<T>(int count)
        {
            for (var j = 0; j < count; j++)
            {
                yield return Create<T>();
            }
        }
    }
}
