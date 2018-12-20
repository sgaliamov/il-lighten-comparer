using System;
using System.Collections.Generic;
using ILLightenComparer.Tests.Samples;

namespace ILLightenComparer.Tests.ComparerTests.ComparableTests.Samples
{
    public class ContainerObject
    {
        public SampleComparableChildObject ComparableField;
        public SampleComparableStruct<EnumBig> ComparableStructField;
        public SampleComparableStruct<EnumSmall>? ComparableStructNullableField;

        public static IComparer<ContainerObject> Comparer { get; } = new RelationalComparer();

        public SampleComparableObject ComparableProperty { get; set; }
        public SampleComparableStruct<decimal>? ComparableStructNullableProperty { get; set; }
        public SampleComparableStruct<string> ComparableStructProperty { get; set; }

        private sealed class RelationalComparer : IComparer<ContainerObject>
        {
            public int Compare(ContainerObject x, ContainerObject y)
            {
                if (ReferenceEquals(x, y)) { return 0; }

                if (ReferenceEquals(null, y)) { return 1; }

                if (ReferenceEquals(null, x)) { return -1; }

                var compare = Comparer<SampleComparableObject>.Default.Compare(
                    x.ComparableField,
                    y.ComparableField);
                if (compare != 0) { return compare; }

                compare = x.ComparableStructField.CompareTo(y.ComparableStructField);
                if (compare != 0) { return compare; }

                compare = Nullable.Compare(
                    x.ComparableStructNullableField,
                    y.ComparableStructNullableField);
                if (compare != 0) { return compare; }

                compare = Comparer<SampleComparableObject>.Default.Compare(
                    x.ComparableProperty,
                    y.ComparableProperty);
                if (compare != 0) { return compare; }

                compare = Nullable.Compare(
                    x.ComparableStructNullableProperty,
                    y.ComparableStructNullableProperty);
                if (compare != 0) { return compare; }

                compare = x.ComparableStructProperty.CompareTo(y.ComparableStructProperty);
                if (compare != 0) { return compare; }

                return compare;
            }
        }
    }
}
