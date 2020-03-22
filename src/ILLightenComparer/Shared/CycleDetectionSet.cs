using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace ILLightenComparer.Shared
{
    internal sealed class CycleDetectionSet : ConcurrentDictionary<object, byte>
    {
        public static readonly ConstructorInfo DefaultConstructor = typeof(CycleDetectionSet).GetConstructor(Type.EmptyTypes);

        public static readonly MethodInfo TryAddMethod = typeof(CycleDetectionSet).GetMethod(nameof(TryAdd), new[] { typeof(object), typeof(byte) });

        public static readonly MethodInfo GetCountProperty = typeof(CycleDetectionSet).GetProperty(nameof(Count)).GetGetMethod();
    }
}
