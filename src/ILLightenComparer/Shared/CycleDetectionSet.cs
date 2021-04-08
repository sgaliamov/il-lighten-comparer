using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using ILLightenComparer.Extensions;
using Illuminator;
using static ILLightenComparer.Extensions.Functions;

namespace ILLightenComparer.Shared
{
    internal sealed class CycleDetectionSet : ConcurrentDictionary<object, byte>
    {
        public static readonly ConstructorInfo DefaultConstructor = typeof(CycleDetectionSet).GetConstructor(Type.EmptyTypes);

        public static ILEmitterFunc Remove(ushort set, ushort arg, Type argType) => CallMethod(
            RemoveMethod,
            Ldarg(set),
            Ldarg(arg) + EmitIf(argType.IsValueType, Box(argType)));

        public static ILEmitterFunc TryAdd(ushort set, ushort arg, Type argType) => CallMethod(
            TryAddMethod,
            Ldarg(set),
            Ldarg(arg) + EmitIf(argType.IsValueType, Box(argType)),
            Ldc_I4(0));

        public static ILEmitterFunc GetCount(ushort arg) => CallMethod(GetCountProperty, Ldarg(arg));

        internal static readonly MethodInfo GetCountProperty = typeof(CycleDetectionSet).GetProperty(nameof(Count)).GetGetMethod();

        private static readonly MethodInfo RemoveMethod = typeof(IDictionary).FindMethod(nameof(IDictionary.Remove), new[] { typeof(object) });

        private static readonly MethodInfo TryAddMethod = typeof(CycleDetectionSet).GetMethod(nameof(TryAdd), new[] { typeof(object), typeof(byte) });
    }
}
