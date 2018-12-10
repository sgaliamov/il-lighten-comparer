﻿using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.BasicMembersTests
{
    public class SampleObject
    {
        public EnumBig EnumField;
        public int Field;

        public static IComparer<SampleObject> Comparer { get; } =
            new ComparableSampleObjectRelationalComparer();

        public EnumBig EnumProperty { get; set; }
        public int Property { get; set; }

        public override string ToString()
        {
            return $"{nameof(EnumField)}: {EnumField}, {nameof(Field)}: {Field}, {nameof(EnumProperty)}: {EnumProperty}, {nameof(Property)}: {Property}";
        }

        private sealed class ComparableSampleObjectRelationalComparer : IComparer<SampleObject>
        {
            public int Compare(SampleObject x, SampleObject y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (ReferenceEquals(null, y))
                {
                    return 1;
                }

                if (ReferenceEquals(null, x))
                {
                    return -1;
                }

                var enumFieldComparison = x.EnumField.CompareTo(y.EnumField);
                if (enumFieldComparison != 0)
                {
                    return enumFieldComparison;
                }

                var fieldComparison = x.Field.CompareTo(y.Field);
                if (fieldComparison != 0)
                {
                    return fieldComparison;
                }

                var enumPropertyComparison = x.EnumProperty.CompareTo(y.EnumProperty);
                if (enumPropertyComparison != 0)
                {
                    return enumPropertyComparison;
                }

                return x.Property.CompareTo(y.Property);
            }
        }
    }
}
