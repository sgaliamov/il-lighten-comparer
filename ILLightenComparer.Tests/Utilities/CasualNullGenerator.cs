using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture.Kernel;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class CasualNullGenerator : ISpecimenBuilder
    {
        private readonly HashSet<Type> _exclude;
        private readonly double _probability;
        private readonly Random _random;

        public CasualNullGenerator(double probability, params Type[] exclude)
        {
            _exclude = new HashSet<Type>(exclude);
            _probability = probability;
            _random = new Random();
        }

        public object Create(object request, ISpecimenContext context)
        {
            var property = request as PropertyInfo;
            var field = request as FieldInfo;

            var owner = property?.DeclaringType ?? field?.DeclaringType;
            var type = property?.PropertyType ?? field?.FieldType;
            if (type == null || owner == null)
            {
                return new NoSpecimen();
            }

            if ((type.IsNullable() || type.IsClass)
                && !_exclude.Contains(owner)
                && _random.NextDouble() < _probability)
            {
                return new OmitSpecimen();
            }

            return new NoSpecimen();
        }
    }
}
