using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ILLightenComparer.Variables;
using Illuminator;
using Illuminator.Extensions;
using static Illuminator.Functional;

namespace ILLightenComparer.Extensions
{
    internal static class ILEmitterExtensions
    {
        private const string LengthMethodName = nameof(Array.Length);
        private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));
        private static readonly MethodInfo GetComparerMethod = typeof(IComparerProvider).GetMethod(nameof(IComparerProvider.GetComparer));
        private static readonly MethodInfo DisposeMethod = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose), Type.EmptyTypes);

        public static ILEmitter EmitArrayLength(this ILEmitter il, Type arrayType, LocalBuilder array, out LocalBuilder count) => il
            .LoadLocal(array)
            .Call(arrayType.GetPropertyGetter(LengthMethodName))
            .Store(typeof(int), out count);

        public static ILEmitter EmitArraySorting(this ILEmitter il, bool hasCustomComparer, Type elementType, params LocalBuilder[] arrays)
        {
            var useSimpleSorting = !hasCustomComparer && elementType.GetUnderlyingType().ImplementsGenericInterface(typeof(IComparable<>));

            if (useSimpleSorting) {
                foreach (var array in arrays) {
                    // todo: 2. compare default sorting and sorting with generated comparer - TrySZSort can work faster
                    EmitSortArray(il, elementType, array);
                }
            } else {
                var getComparerMethod = GetComparerMethod.MakeGenericMethod(elementType);

                il.LoadArgument(Arg.Context)
                  .Call(getComparerMethod)
                  .Store(getComparerMethod.ReturnType, out var comparer);

                foreach (var array in arrays) {
                    EmitSortArray(il, elementType, array, comparer);
                }
            }

            return il;
        }

        public static ILEmitter EmitDispose(this ILEmitter il, LocalBuilder local) => il
            .LoadCaller(local)
            .ExecuteIf(local.LocalType.IsValueType, Constrained(local.LocalType))
            .Call(DisposeMethod);

        private static void EmitSortArray(ILEmitter il, Type elementType, LocalBuilder array, LocalBuilder comparer)
        {
            var copyMethod = ToArrayMethod.MakeGenericMethod(elementType);
            var sortMethod = GetArraySortWithComparer(elementType);

            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .LoadLocal(comparer)
              .Call(sortMethod);
        }

        private static void EmitSortArray(ILEmitter il, Type elementType, LocalBuilder array)
        {
            var copyMethod = ToArrayMethod.MakeGenericMethod(elementType);
            var sortMethod = GetArraySortMethod(elementType);

            il.LoadLocal(array)
              .Call(copyMethod)
              .Store(array)
              .LoadLocal(array)
              .Call(sortMethod);
        }

        private static MethodInfo GetArraySortWithComparer(Type elementType) => typeof(Array)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == nameof(Array.Sort) && x.IsGenericMethodDefinition)
            .Single(x => {
                var parameters = x.GetParameters();

                return parameters.Length == 2
                    && parameters[0].ParameterType.IsArray
                    && parameters[1].ParameterType.IsGenericType
                    && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(IComparer<>);
            })
            .MakeGenericMethod(elementType);

        private static MethodInfo GetArraySortMethod(Type elementType) => typeof(Array)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == nameof(Array.Sort) && x.IsGenericMethodDefinition)
            .Single(x => {
                var parameters = x.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType.IsArray;
            })
            .MakeGenericMethod(elementType);
    }
}
