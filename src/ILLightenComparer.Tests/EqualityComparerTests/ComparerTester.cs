//using System.Collections.Generic;
//using System.Linq;
//using AutoFixture;
//using AutoFixture.Kernel;
//using FluentAssertions;
//using FluentAssertions.Execution;
//using Force.DeepCloner;
//using ILLightenComparer.Tests.Utilities;

//namespace ILLightenComparer.Tests.EqualityComparerTests
//{
//    internal static class ComparerTester
//    {
//        public static void TestAllFields<T>(IEqualityComparer<T> comparer)
//        {
//            TestAllFields(comparer, new string[0]);
//        }

//        public static void TestAllFields<T>(IEqualityComparer<T> comparer, string[] propertiesToIgnore)
//        {
//            var fixture = FixtureBuilder.GetInstance();
//            var context = new SpecimenContext(fixture);

//            var changes = typeof(T)
//                          .GetProperties()
//                          .Where(x => !propertiesToIgnore.Contains(x.Name))
//                          .Select(
//                              property =>
//                              {
//                                  var one = fixture.Create<T>();
//                                  var other = one.DeepClone();

//                                  var oldValue = property.GetValue(one);
//                                  var newValue = property.PropertyType == typeof(bool)
//                                                     ? !(bool)oldValue
//                                                     : context.Resolve(property.PropertyType);
//                                  property.SetValue(other, newValue);

//                                  return new
//                                  {
//                                      One = one,
//                                      Other = other,
//                                      PropertyName = property.Name,
//                                      OldValue = oldValue,
//                                      NewValue = newValue
//                                  };
//                              });

//            // act
//            var results = changes
//                          .Select(x => new
//                          {
//                              ObjectsAreEquals = comparer.Equals(x.One, x.Other),
//                              HashCodesAreEqual = comparer.GetHashCode(x.One) == comparer.GetHashCode(x.Other),
//                              ErrorMessage = $"Property [{x.PropertyName}] is wrongly compared. "
//                                             + $"Old value: {x.OldValue}, new value: {x.NewValue}."
//                          })
//                          .ToArray();

//            // assert
//            using (new AssertionScope())
//            {
//                foreach (var item in results)
//                {
//                    item.ObjectsAreEquals.Should().BeFalse(item.ErrorMessage);
//                    item.HashCodesAreEqual.Should().BeFalse(item.ErrorMessage);
//                }
//            }
//        }

//        public static void PositiveTest<T>(IEqualityComparer<T> comparer)
//        {
//            PositiveTest(comparer, new string[0]);
//        }

//        public static void PositiveTest<T>(IEqualityComparer<T> comparer, string[] propertiesToIgnore)
//        {
//            var fixture = FixtureBuilder.GetInstance();

//            var one = fixture.Create<T>();
//            var other = one.DeepClone();

//            MutateProperties(propertiesToIgnore, other);

//            // act
//            var result = comparer.Equals(one, other);
//            var oneHashCode = comparer.GetHashCode(one);
//            var otherHashCode = comparer.GetHashCode(other);

//            // assert
//            result.Should().BeTrue();
//            oneHashCode.Should().Be(otherHashCode);
//        }

//        private static void MutateProperties<T>(string[] propertiesToIgnore, T other)
//        {
//            var fixture = FixtureBuilder.GetInstance();
//            var context = new SpecimenContext(fixture);

//            typeof(T)
//                .GetProperties()
//                .Where(x => propertiesToIgnore.Contains(x.Name))
//                .ToList()
//                .ForEach(property =>
//                {
//                    var newValue = property.PropertyType == typeof(bool)
//                                       ? !(bool)property.GetValue(other)
//                                       : context.Resolve(property.PropertyType);

//                    property.SetValue(other, newValue);
//                });
//        }
//    }
//}
