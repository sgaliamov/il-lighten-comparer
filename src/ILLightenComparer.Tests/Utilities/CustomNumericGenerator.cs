using System;
using System.Reflection;
using AutoFixture.Kernel;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class CustomNumericGenerator : ISpecimenBuilder
    {
        private readonly int _lower;
        private readonly double _minMaxProbability;
        private readonly int _upper;

        public CustomNumericGenerator(int lower, int upper, double minMaxProbability)
        {
            _lower = lower;
            _upper = upper;
            _minMaxProbability = minMaxProbability;
        }

        public object Create(object request, ISpecimenContext context) => request is Type type
            ? CreateRandom(type)
            : new NoSpecimen();

        private object CreateRandom(Type request) => (Type.GetTypeCode(request)) switch
        {
            TypeCode.Byte => MinMax<byte>(request) ?? (byte)GetNextRandom(),
            TypeCode.Decimal => MinMax<decimal>(request) ?? GetNextRandom(),
            TypeCode.Double => MinMax<double>(request) ?? GetNextRandom(),
            TypeCode.Int16 => MinMax<short>(request) ?? (short)GetNextRandom(),
            TypeCode.Int32 => MinMax<int>(request) ?? (int)GetNextRandom(),
            TypeCode.Int64 => MinMax<long>(request) ?? GetNextRandom(),
            TypeCode.SByte => MinMax<sbyte>(request) ?? (sbyte)GetNextRandom(),
            TypeCode.Single => MinMax<float>(request) ?? GetNextRandom(),
            TypeCode.UInt16 => MinMax<ushort>(request) ?? (ushort)GetNextRandom(),
            TypeCode.UInt32 => MinMax<uint>(request) ?? (uint)GetNextRandom(),
            TypeCode.UInt64 => MinMax<ulong>(request) ?? (ulong)GetNextRandom(),
            _ => new NoSpecimen(),
        };

        private T? MinMax<T>(IReflect request) where T : struct
        {
            if (ThreadSafeRandom.NextDouble() >= _minMaxProbability) {
                return null;
            }

            object Get(IReflect t, string name) => t.GetField(name, BindingFlags.Static | BindingFlags.Public).GetValue(null);

            return ThreadSafeRandom.NextDouble() < 0.5
                ? (T)Get(request, "MinValue")
                : (T)Get(request, "MaxValue");
        }

        private long GetNextRandom() => ThreadSafeRandom.Next(_lower, _upper);
    }
}
