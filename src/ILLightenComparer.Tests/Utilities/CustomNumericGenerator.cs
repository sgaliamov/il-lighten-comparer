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

        public object Create(object request, ISpecimenContext context)
        {
            return request is Type type
                       ? CreateRandom(type)
                       : new NoSpecimen();
        }

        private object CreateRandom(Type request)
        {
            switch (Type.GetTypeCode(request))
            {
                case TypeCode.Byte:
                    return MinMax<byte>(request) ?? (byte)GetNextRandom();

                case TypeCode.Decimal:
                    return MinMax<decimal>(request) ?? GetNextRandom();

                case TypeCode.Double:
                    return MinMax<double>(request) ?? GetNextRandom();

                case TypeCode.Int16:
                    return MinMax<short>(request) ?? (short)GetNextRandom();

                case TypeCode.Int32:
                    return MinMax<int>(request) ?? (int)GetNextRandom();

                case TypeCode.Int64:
                    return MinMax<long>(request) ?? GetNextRandom();

                case TypeCode.SByte:
                    return MinMax<sbyte>(request) ?? (sbyte)GetNextRandom();

                case TypeCode.Single:
                    return MinMax<float>(request) ?? GetNextRandom();

                case TypeCode.UInt16:
                    return MinMax<ushort>(request) ?? (ushort)GetNextRandom();

                case TypeCode.UInt32:
                    return MinMax<uint>(request) ?? (uint)GetNextRandom();

                case TypeCode.UInt64:
                    return MinMax<ulong>(request) ?? (ulong)GetNextRandom();

                default: return new NoSpecimen();
            }
        }

        private T? MinMax<T>(IReflect request) where T : struct
        {
            if (ThreadSafeRandom.NextDouble() >= _minMaxProbability)
            {
                return null;
            }

            object Get(IReflect t, string name)
            {
                return t.GetField(name, BindingFlags.Static | BindingFlags.Public).GetValue(null);
            }

            return ThreadSafeRandom.NextDouble() < 0.5
                       ? (T)Get(request, "MinValue")
                       : (T)Get(request, "MaxValue");
        }

        private long GetNextRandom()
        {
            return ThreadSafeRandom.Next(_lower, _upper);
        }
    }
}
