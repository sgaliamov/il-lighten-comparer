using System;
using System.Reflection;
using AutoFixture.Kernel;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class NumericGenerator : ISpecimenBuilder
    {
        private readonly int _lower;
        private readonly sbyte _minMaxProbability;
        private readonly Random _random;
        private readonly int _upper;

        public NumericGenerator(int lower, int upper, sbyte minMaxProbability)
        {
            _lower = lower;
            _upper = upper;
            _minMaxProbability = minMaxProbability;
            _random = new Random();
        }

        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;
            if (type == null)
            {
                return new NoSpecimen();
            }

            return CreateRandom(type);
        }

        private object CreateRandom(Type request)
        {
            switch (Type.GetTypeCode(request))
            {
                case TypeCode.Byte:
                    return (byte)GetNextRandom();

                case TypeCode.Decimal:
                    return (decimal)GetNextRandom();

                case TypeCode.Double:
                    return (double)GetNextRandom();

                case TypeCode.Int16:
                    return (short)GetNextRandom();

                case TypeCode.Int32:
                    return (int)GetNextRandom();

                case TypeCode.Int64:
                    return GetNextRandom();

                case TypeCode.SByte:
                    return (sbyte)GetNextRandom();

                case TypeCode.Single:
                    return (float)GetNextRandom();

                case TypeCode.UInt16:
                    return (ushort)GetNextRandom();

                case TypeCode.UInt32:
                    return (uint)GetNextRandom();

                case TypeCode.UInt64:
                    return (ulong)GetNextRandom();

                default: return new NoSpecimen();
            }
        }

        private object MinMax(IReflect request)
        {
            if (!(_random.NextDouble() < _minMaxProbability))
            {
                return null;
            }

            object Get(IReflect t, string name) =>
                t.GetField(name, BindingFlags.Static | BindingFlags.Public).GetValue(null);

            return _random.NextDouble() < 0.5
                ? Get(request, "MinValue")
                : Get(request, "MaxValue");
        }

        private long GetNextRandom()
        {
            long result;
            if (_lower >= int.MinValue && _upper <= int.MaxValue)
            {
                result = _random.Next((int)_lower, (int)_upper);
            }
            else
            {
                result = GetNextInt64InRange();
            }

            return result;
        }

        private long GetNextInt64InRange()
        {
            var range = (ulong)(_upper - _lower);
            var limit = ulong.MaxValue - ulong.MaxValue % range;
            ulong number;
            do
            {
                var buffer = new byte[sizeof(ulong)];
                _random.NextBytes(buffer);
                number = BitConverter.ToUInt64(buffer, 0);
            } while (number > limit);

            return (long)(number % range + (ulong)_lower);
        }
    }
}
